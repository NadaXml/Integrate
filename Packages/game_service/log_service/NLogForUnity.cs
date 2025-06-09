using NLog;
using NLog.Targets;
using UnityEngine;
namespace game_logic.log_service {
    [Target("UnityConsole")]
    public class NLogForUnity : Target {
        public string ConnectionString { get; set; }

        protected override void Write(LogEventInfo logEvent) {
            base.Write(logEvent);

            string logMessage = $"{logEvent.TimeStamp} [{logEvent.Level}] {logEvent.Message}";
            Debug.Log(logMessage);
        }
    }
}
