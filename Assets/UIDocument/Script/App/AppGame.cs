using AppFrame;
using UIFrame.Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.Service;
using UIDocument.Script.System;
using UnityEngine;
using YooAsset;
using IServiceProvider = AppFrame.IServiceProvider;
using NotImplementedException = System.NotImplementedException;

namespace UIDocument.Script.App {
    public class AppGame : IApp, IServiceProvider
    {
        public enum ServiceType {
            Asset = 0,
            Scene = 1,
            UI = 2,
        }

        public enum SystemType {
            UI = 0,
        }

        Dictionary<int, IService> _services;
        Dictionary<int, ISystem> _systems;

        AppContext _appContext;
        
        EPlayMode _playMode = EPlayMode.OfflinePlayMode;
        
        public void Awake() {
            _services = new Dictionary<int, IService>();
            _systems = new Dictionary<int, ISystem>();
            _appContext = new AppContext();
            _appContext.ServiceContext = new ServiceContext();
            _appContext.SystemContext = new SystemContext(this);
        }
        public void Destroy() {
            foreach (var pair in _services) {
                pair.Value.Destroy();
            }
        }
        public IEnumerator Start() {
            IService service = new AssetService.AssetService("DefaultPackage", _playMode);
            yield return service.Start();
            _services.Add((int)ServiceType.Asset, service);

            service = new SceneService.SceneService();
            yield return service.Start();
            _services.Add((int)ServiceType.Scene, service);
            
            // ISystem system = new UISystem(this);
            // yield return system.Start();
            // _systems.Add((int)SystemType.UI, system);
        }
        public async void Play() {
            IService service;
            _services.TryGetValue((int)ServiceType.Scene, out service);
            ISystem system;
            _systems.TryGetValue((int)SystemType.UI, out system);
            
            // UISystem uiSystem = system as UISystem;
            
            SceneService.SceneService sceneService = service as SceneService.SceneService;
            await sceneService.LoadSceneAsync(_appContext.ServiceContext, "HUD.unity").ToUniTask();
            Debug.Log("场景加载成功");
            
        }

        public void SetPlayMode(EPlayMode playMode) {
            _playMode = playMode;
        }
        
        public T GetService<T>(int serviceType) where T : class, IService {
            _services.TryGetValue(serviceType, out IService service);
            return service as T;
        }
        public T GetSystem<T>(int systemType) where T : class, ISystem {
            _systems.TryGetValue(systemType, out ISystem system);
            return system as T;
        }
    }
}
