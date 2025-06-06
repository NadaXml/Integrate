using adt;
using asset_service;
using Cysharp.Threading.Tasks;
using data_module;
using game_fund;
using game_logic.system;
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
        
        async UniTask<GameProcedure> CreateSystems(CancellationTokenSource cancelSource) {
            GameProcedure ret = GameProcedure.None; 
            gameContext.missionSystem = CreateSystem<Mission>();
            ret = GameProcedure.Success;
            return ret;
        }
        
        void DestroySystems() {
            DestroySystem(gameContext.missionSystem);
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
            service.Awake();
            return service;
        }

        
        async UniTask<GameProcedure> CreateServices(CancellationTokenSource cts) {
            GameProcedure ret = GameProcedure.None;
            var assetService = CreateService<Asset>();
            ret = await assetService.Init(gameContext.assetParam, cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            gameContext.assetService = assetService;
            return ret;
        }
        
        void DestroyServices() {
            DestroyService(gameContext.assetService);
        }
        
  #endregion
        
        #region GameModule

        void DestroyModule(IModule module) {
            gameContext.UnRegisterModule(module);
            module.Destroy();
        }

        T CreateModule<T>() where T : GameModule, new() {
            T module = new T();
            gameContext.RegisterModule(module);
            module.Awake();
            return module;
        }

        async UniTask<GameProcedure> CreateModule(CancellationToken cts) {
            GameProcedure ret = GameProcedure.None;
            var dataModule = CreateModule<Data>();
            ret = await dataModule.Init(cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            gameContext.dataModule = dataModule;
            return ret;
        }
        
        #endregion

        CancellationTokenSource ctsForStart;
        
        public async UniTask<GameProcedure> Start(Asset.AssetParam assetParam) {

            ctsForStart = new CancellationTokenSource();
            
            GameProcedure ret = GameProcedure.None;
            CreateGameContext();
            gameContext.assetParam = assetParam;
            ret = await CreateServices(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            ret = await CreateSystems(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            Debug.Log("crate mission frame " + Time.frameCount);
            ret = await gameContext.missionSystem.CreateMission();
            Debug.Log("crate mission over frame" + Time.frameCount);

            gameContext.procedure = ret;
            return ret;
        }

        public void End() {
            ctsForStart.Cancel();

            if (gameContext.missionSystem != null) {
                gameContext.missionSystem.DestroyMission();
            }
            
            DestroySystems();
            DestroyServices();
            DestroyGameContext();
        }

        public void Tick() {
            
        }

        public void SetFrameCount(int frameCount) {
            gameContext.runParam.frameCount = frameCount;
        }

        public GameContext.RunParam GetRunParam() {
            return gameContext.runParam;
        }

        public GameProcedure GetGameProcedure() {
            return gameContext.procedure;
        }
    }
}
