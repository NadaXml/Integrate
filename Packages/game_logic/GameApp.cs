using adt;
using asset_service;
using Cysharp.Threading.Tasks;
using game_fund;
using game_logic.system;
using System.Threading;

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
            CreateSystem<Mission>();
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
            ret = await assetService.Init(gameContext.AssetParam, cts);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            return ret;
        }
        
        void DestroyServices() {
            DestroyService(gameContext.assetService);
        }
        
  #endregion

        CancellationTokenSource ctsForStart;
        
        public async UniTask<GameProcedure> Start(Asset.AssetParam assetParam) {

            ctsForStart = new CancellationTokenSource();
            
            GameProcedure ret = GameProcedure.None;
            CreateGameContext();
            gameContext.AssetParam = assetParam;
            ret = await CreateServices(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }
            ret = await CreateSystems(ctsForStart);
            if (ret != GameProcedure.Success) {
                return ret;
            }

            gameContext.missionSystem.CreateMission();
            
            return ret;
        }

        public void End() {
            ctsForStart.Cancel();
            DestroySystems();
            DestroyServices();
            DestroyGameContext();
        }
    }
}
