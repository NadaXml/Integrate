using AppFrame;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.Module;
using UIFrame.Core;
using YooAsset;

namespace UIDocument.Script.App {
    public class AppGame : IApp
    {
        List<IService> _services;
        List<ISystem> _systems;
        
        EPlayMode _playMode = EPlayMode.OfflinePlayMode;

        AppContext _appContext;
        
        StartUp _startUp;
        
        public void Awake() {
            _services = new List<IService>();
            _systems = new List<ISystem>();
            _appContext = new AppContext();
        }
        public void Destroy() {
            
            _startUp.Destroy();
            
            if (_startUp != null) {
                _startUp.Destroy();
                _startUp = null;
            } 
            foreach (IService service in _services) {
                service.Destroy();
            }
            _services.Clear();

            foreach (ISystem system in _systems) {
                system.Destroy();
            }
            _systems.Clear();
        }
        public IEnumerator Start() {
            
            // 初始化服务
            AssetService.AssetService assetService = new AssetService.AssetService("DefaultPackage", _playMode);
            assetService.Awake();
            yield return assetService.Start();
            _services.Add(assetService);

            SceneSystem.SceneSystem.CreateParameters sceneCreateParam = new SceneSystem.SceneSystem.CreateParameters() {
                AssetService = assetService,
            };
            SceneSystem.SceneSystem sceneSystem = new SceneSystem.SceneSystem(in sceneCreateParam);
            sceneSystem.Awake();
            yield return sceneSystem.Start();
            _systems.Add(sceneSystem);

            UISystem.CrateParam uiCreateParam = new UISystem.CrateParam() {
                _assetProvider = assetService
            };
            
            // 初始化系统
            UISystem uiSystem = new UISystem(in uiCreateParam);
            uiSystem.Awake();
            yield return uiSystem.Start();
            _systems.Add(uiSystem);

            var createParam = new StartUp.CreateParam() {
                StartUpContext = new StartUp.Context() {
                    SceneSystem = sceneSystem,
                    AssetService = assetService,
                    UISystem = uiSystem
                },
                AppContext = _appContext
            };
            _startUp = new StartUp(in createParam);
        }
        public void Update(float deltaTime) {
            _systems.ForEach(system => system.Update(deltaTime));
        }
        
        public void Play() {
            _startUp.Play();
        }

        public void SetPlayMode(EPlayMode playMode) {
            _playMode = playMode;
        }
    }
}
