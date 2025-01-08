

using AppFrame;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.Service;
using UnityEngine;
using YooAsset;
namespace UIDocument.Script.App {
    public class AppGame : IApp
    {
        public enum ServiceType {
            Asset = 0,
            Scene = 1,
        }

        public enum SystemType {
            UI = 0,
        }

        Dictionary<ServiceType, IService> _services;
        IServiceContext _context;
        
        Dictionary<SystemType, ISystem> _systems;
        ISystemContext _systemContext;
        
        EPlayMode _playMode = EPlayMode.OfflinePlayMode;
        
        public void Awake() {
            _services = new Dictionary<ServiceType, IService>();
            _context = new ServiceContext();
        }
        public void Destroy() {
            foreach (var pair in _services) {
                pair.Value.Destroy();
            }
        }
        public IEnumerator Start() {
            IService service = new AssetService.AssetService("DefaultPackage", _playMode);
            yield return service.Start();
            _services.Add(ServiceType.Asset, service);

            service = new SceneService.SceneService();
            yield return service.Start();
            _services.Add(ServiceType.Scene, service);
        }
        public async void Play() {
            IService service;
            if (_services.TryGetValue(ServiceType.Scene, out service)) {
                SceneService.SceneService sceneService = service as SceneService.SceneService;
                await sceneService.LoadSceneAsync(_context, "HUD.unity").ToUniTask();
                Debug.Log("场景加载成功");
            }
        }

        public void SetPlayMode(EPlayMode playMode) {
            _playMode = playMode;
        }
    }
}
