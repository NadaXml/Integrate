using adt;
using asset_service;
using Cysharp.Threading.Tasks;
using game_logic.module;
using game_fund;
using game_logic.system;
using game_service;
using game_service.pool_service;
using log_service;
using System;
using System.Threading;
using UnityEngine;

namespace game_logic {
    public class GameApp : App {
        
        #region GameContext
        
        GameContext gameContext;
        
        void CreateGameContext() {
            gameContext = new GameContext();
            gameContext.Awake();
        }

        void DestroyGameContext() {
            gameContext?.Destroy();
            gameContext = null;
        }
        
  #endregion

        #region GameSystem
        
        T CreateSystem<T>() where T : GameSystem, new() {
            T system = new T();
            system.Awake();
            gameContext.RegisterSystem(system);
            system.BindGameContext(gameContext);
            return system;
        }
        
        void DestroySystem(ISystem system) {
            gameContext.UnRegisterSystem(system);
            system.Destroy();
        }
        
        async UniTask<GameProcedure> CreateSystems(CancellationTokenSource _) {
            GameProcedure ret = GameProcedure.None; 
            gameContext.missionSystem = CreateSystem<Mission>();
            gameContext.roundSystem = CreateSystem<Round>();
            gameContext.debugConsoleSystem = CreateSystem<DebugConsole>();
            ret = GameProcedure.Success;
            return ret;
        }
        
        void DestroySystems() {
            foreach (WeakReference<ISystem> weakRef in gameContext.systems) {
                if (weakRef.TryGetTarget(out ISystem system)) {
                    DestroySystem(system);
                }
            }
        }
        
  #endregion

        #region GameService
        
        void DestroyService(IService service) {
            gameContext.UnRegisterService(service);
            service.Destroy();
        }
        
        T CreateService<T>() where T : GameService, new() {
            T service = new T();
            gameContext.RegisterService(service);
            service.SetFund(gameContext);
            service.Awake();
            return service;
        }
        
        async UniTask<GameProcedure> CreateServices(CancellationTokenSource cts) {
            GameProcedure ret = GameProcedure.None;
            
            var logService = CreateService<NLogService>();
            ret = await logService.Open(gameContext.appconfig.nLogParam);
            gameContext.logService = logService;
            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            var assetService = CreateService<Asset>();
            ret = await assetService.Init(gameContext.appconfig.assetParam, cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            gameContext.assetService = assetService;

            var poolService = CreateService<PoolService>();
            ret = await poolService.Init(cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            gameContext.poolService = poolService;
            
            return ret;
        }
        
        void DestroyServices() {
            foreach (var weakRef in gameContext.services) {
                DestroyService(gameContext.assetService);
            }
        }
        
  #endregion
        
        #region GameModule

        void DestroyModules() {
            foreach (var weakRef in gameContext.modules) {
                if (weakRef.TryGetTarget(out IModule module)) {
                    DestroyModule(module);
                }
            }
        }
        
        void DestroyModule(IModule module) {
            gameContext.UnRegisterModule(module);
            module.Destroy();
        }

        T CreateModule<T>() where T : GameModule, new() {
            T module = new T();
            gameContext.RegisterModule(module);
            module.BindGameContext(gameContext);
            module.Awake();
            return module;
        }

        async UniTask<GameProcedure> CreateModules(CancellationTokenSource cts) {
            GameProcedure ret = GameProcedure.Success;
            var dataModule = CreateModule<Data>();
            ret = await dataModule.Init(cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            gameContext.dataModule = dataModule;
            var eventModule = CreateModule<EventCenter>();
            ret = await eventModule.Init(cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            gameContext.eventModule = eventModule;
            return ret;
        }
        
        #endregion

        CancellationTokenSource ctsForStart;
        
        public async UniTask<GameProcedure> Start(GameContext.AppConfig appconfig) {

            ctsForStart = new CancellationTokenSource();
            
            GameProcedure ret = GameProcedure.None;
            CreateGameContext();
            gameContext.appconfig = appconfig;
            ret = await CreateServices(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }

            ret = await CreateModules(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            ret = await CreateSystems(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            gameContext.logService.logger.Info($"crate mission frame {Time.frameCount}");
            ret = await gameContext.missionSystem.CreateMission();
            gameContext.logService.logger.Info($"crate mission over frame {Time.frameCount}");
            gameContext.procedure = ret;
            gameContext.logService.logger.Info($"create mission ret {ret}");
            
            return ret;
        }

        public void End() {
            ctsForStart.Cancel();

            if (gameContext.missionSystem != null) {
                gameContext.missionSystem.DestroyMission();
            }
            
            DestroySystems();
            DestroyModules();
            DestroyServices();
            DestroyGameContext();
        }

        public void Tick() {
            foreach (var system in gameContext.systems) {
                if (system.TryGetTarget(out ISystem target)) {
                    target.Update();
                }
            }
        }

        public void StepFrameCount(int delta) {
            gameContext.runParam.frameCount += delta;
        }

        public GameContext.AppConfig GetAppConfig() {
            return gameContext.appconfig;
        }

        public GameProcedure GetGameProcedure() {
            return gameContext.procedure;
        }
    }
}
