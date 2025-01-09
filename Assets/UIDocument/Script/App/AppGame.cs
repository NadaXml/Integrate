using AppFrame;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.Module;
using UIDocument.Script.Service;
using UIDocument.Script.System;
using UIFrame.Core;
using YooAsset;

namespace UIDocument.Script.App {
    public class AppGame : IApp
    {
        List<IService> _services;
        List<ISystem> _systems;

        AppContext _appContext;
        
        EPlayMode _playMode = EPlayMode.OfflinePlayMode;
        
        StartUp _startUp;
        
        public void Awake() {
            _services = new List<IService>();
            _systems = new List<ISystem>();
            _appContext = new AppContext();
            _appContext.ServiceContext = new ServiceContext();
            _appContext.SystemContext = new SystemContext();
        }
        public void Destroy() {
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
            yield return assetService.Start();
            _services.Add(assetService);

            SceneService.SceneService sceneService = new SceneService.SceneService();
            yield return sceneService.Start();
            _services.Add(sceneService);

            UISystem.CrateParam uiCreateParam = new UISystem.CrateParam() {
                _assetProvider = assetService
            };
            // 初始化系统
            ISystem uiSystem = new UISystem(in uiCreateParam);
            yield return uiSystem.Start();
            _systems.Add(uiSystem);

            var createParam = new StartUp.CreateParam();
            _startUp = new StartUp(in createParam);
        }
        public void Play() {
            _startUp.Play();
        }

        public void SetPlayMode(EPlayMode playMode) {
            _playMode = playMode;
        }
    }
}
