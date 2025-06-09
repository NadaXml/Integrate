using adt;
using Cysharp.Threading.Tasks;
using NLog;
using NLog.Config;

namespace game_logic.log_service {
    public class NLogService : GameService {

        public struct NLogParam {
            
        }

        NLogParam nlogParam;

        public NLog.Logger logger;
        
        public async UniTask<GameProcedure> Open(NLogParam nlogParam) {
            this.nlogParam = nlogParam;

            NLog.LogManager.Setup().SetupExtensions(ext => {
                ext.RegisterTarget<NLogForUnity>();
            });

            var config = new LoggingConfiguration();
            var logUnityConsole = new NLogForUnity();
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logUnityConsole);

            NLog.LogManager.Configuration = config;
            
            logger = LogManager.GetCurrentClassLogger();
            logger.Info("NLog 测试输出");
            return GameProcedure.Success;
        }

        public void Close() {
            logger = null;
        }
    }
}
