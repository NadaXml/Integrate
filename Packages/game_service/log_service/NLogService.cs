using adt;
using Cysharp.Threading.Tasks;
using game_service;
using NLog;
using NLog.Targets;
using System;
using UnityEngine;

namespace log_service {
    public class NLogService : GameService {

        [Serializable]
        public struct NLogParam {
            public TextAsset nlogConfig;
        }
        
        public NLog.Logger logger;
        
        public async UniTask<GameProcedure> Open(NLogParam nLogParam) {
            // 注册类型
            NLog.LogManager.Setup().SetupExtensions(ext => {
                ext.RegisterTarget<NLogForUnity>("NLogForUnity");
            });
            
            // 加载配置
            NLog.LogManager.Setup().LoadConfigurationFromXml(nLogParam.nlogConfig.text);
            var configuration = LogManager.Configuration;

            var logfileTarget = configuration.FindTargetByName<FileTarget>("logfile");
            if (logfileTarget != null) {
                logfileTarget.FileName = Application.persistentDataPath + $"/logs/{logfileTarget.Name}_log.log";
            }
            var unityInfo = configuration.FindTargetByName<NLogForUnity>("unityInfo");
            if (unityInfo != null) {
                unityInfo.FileName = Application.persistentDataPath + $"/logs/{unityInfo.Name}_log.log";
            }
            var unityError = configuration.FindTargetByName<NLogForUnity>("unityError");
            if (unityError != null) {
                unityError.FileName = Application.persistentDataPath + $"/logs/{unityError.Name}_log.log";
            }
            LogManager.Configuration = configuration;
            
            // 获取logger
            logger = LogManager.GetCurrentClassLogger();
            return GameProcedure.Success;
        }
        protected override void OnDestroy() {
            logger = null;
            NLog.LogManager.Shutdown();
        }
    }
}
