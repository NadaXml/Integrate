using AppFrame;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.Module;
using UIFrame.Core;
using Unity.VisualScripting;
using YooAsset;

namespace UIDocument.Script.App {
    public class AppGame : IApp, EventService.EventServiceProvider
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
            
            if (_startUp != null) {
                _startUp.Destroy();
                _startUp = null;
            } 
            
            foreach (ISystem system in _systems) {
                system.Destroy();
            }
            _systems.Clear();
            
            foreach (IService service in _services) {
                service.Destroy();
            }
            _services.Clear();
        }
        public IEnumerator Start() {
            
            // 初始化服务
            AssetService.AssetService assetService = new AssetService.AssetService("DefaultPackage", _playMode);
            assetService.Awake();
            yield return assetService.Start();
            _services.Add(assetService);
            
            EventService.EventService eventService = new EventService.EventService();   
            eventService.Awake();
            yield return eventService.Start();
            _services.Add(eventService);
            
            // 初始化系统
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
            
            UISystem uiSystem = new UISystem(in uiCreateParam);
            uiSystem.Awake();
            yield return uiSystem.Start();
            _systems.Add(uiSystem);

            RoundSystem.RoundSystem.CreateParam roundCreateParam = new RoundSystem.RoundSystem.CreateParam() {
                EventServiceProvider = this
            };
            
            RoundSystem.RoundSystem roundSystem = new RoundSystem.RoundSystem(roundCreateParam);
            roundSystem.Awake();
            yield return roundSystem.Start();
            _systems.Add(roundSystem);

            DebugSystem.DebugSystem.CreateParam debugCreateParam = new DebugSystem.DebugSystem.CreateParam() {
                Context = new DebugSystem.DebugSystem.DebugSystemContext() {
                    EventServiceProvider = this
                }
            };
            DebugSystem.DebugSystem debugSystem = new DebugSystem.DebugSystem(in debugCreateParam);
            debugSystem.Awake();
            yield return debugSystem.Start();
            _systems.Add(debugSystem);

            var createParam = new StartUp.CreateParam() {
                StartUpContext = new StartUp.Context() {
                    SceneSystem = sceneSystem,
                    AssetService = assetService,
                    RoundSystem = roundSystem,
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

        EventService.EventService _eventService;
        public EventService.EventService GetEventService() {
            if (_eventService == null) {
                _eventService = _services.Find(service => service is EventService.EventService) as EventService.EventService;
            }
            return _eventService;
        }

        public void SetPlayMode(EPlayMode playMode) {
            _playMode = playMode;
        }
    }
}
