using adt;
using asset_service;
using game_logic.module;
using game_fund;
using game_logic.system;
using game_service;
using game_service.pool_service;
using log_service;
using System;
using System.Collections.Generic;

namespace game_logic {
    public class GameContext : IFunProvider {

        public struct RunParam {
            /// <summary>
            /// 帧率
            /// </summary>
            public int frameRate;
            /// <summary>
            /// 帧计数
            /// </summary>
            public int frameCount;
        }

        [Serializable]
        public struct AppConfig {
            public Asset.AssetParam assetParam;
            public NLogService.NLogParam nLogParam;
            public int frameRate;
        }

        public AppConfig appconfig;
        public RunParam runParam;
        public GameProcedure procedure = GameProcedure.None;
        
        public void Awake() {
            systems = new HashSet<WeakReference<ISystem>>();
            services = new HashSet<WeakReference<IService>>();
            modules = new HashSet<WeakReference<IModule>>();
        }
        public void Destroy() {
            if (systems != null) {
                systems.Clear();
                systems = null;
            }
            if (services != null) {
                services.Clear();
                services = null;
            }
            if (modules != null) {
                modules.Clear();
                modules = null;
            }
        }

        #region Systems
        
        public HashSet<WeakReference<ISystem>> systems;
        public Mission missionSystem;
        public Round roundSystem;
        public DebugConsole debugConsoleSystem;

        /// <summary>
        /// 注册系统
        /// </summary>
        /// <param name="system"></param>
        public void RegisterSystem(ISystem system) {
            // TODO Debug 再防重入
            systems.Add(new WeakReference<ISystem>(system));
        }

        /// <summary>
        /// 反注册系统
        /// </summary>
        /// <param name="system"></param>
        public void UnRegisterSystem(ISystem system) {
            systems.RemoveWhere((weakRef) => {
                if (weakRef.TryGetTarget(out var weakOjb)) {
                    return weakOjb == system;
                }
                return false;
            });
        }
            
        #endregion

        #region Module
        
        public HashSet<WeakReference<IModule>> modules;
        public Data dataModule;
        public EventCenter eventModule;

        public void RegisterModule(IModule module) {
            modules.Add(new WeakReference<IModule>(module));
        }

        public void UnRegisterModule(IModule module) {
            modules.RemoveWhere((weakRef) => {
                if (weakRef.TryGetTarget(out var weakObj)) {
                    return weakObj == module;
                }
                return false;
            });
        }
        
        #endregion
        
        #region Services

        public HashSet<WeakReference<IService>> services;
        public Asset assetService;
        public NLogService logService { get; set; }
        public PoolService poolService { get; set; }
        
        /// <summary>
        /// 注册服务
        /// </summary>
        public void RegisterService(IService service) {
            // TODO debug 防止重入
            services.Add(new WeakReference<IService>(service));
        }

        /// <summary>
        /// 反注册服务
        /// </summary>
        /// <param name="service"></param>
        public void UnRegisterService(IService service) {
            services.RemoveWhere((weakRef) => {
                if (weakRef.TryGetTarget(out var weakObj)) {
                    return weakObj == service;
                }
                return false;
            });
        }

        #endregion
    }
}
