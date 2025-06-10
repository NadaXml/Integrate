using NLog;
using NLog.Common;
using NLog.Targets;
using System.Collections.Generic;
using UnityEngine;
namespace log_service {
    [Target("NLogForUnity")]
    public class NLogForUnity : FileTarget {
        public string ConnectionString { get; set; }

        protected override void Write(LogEventInfo logEvent) {
            base.Write(logEvent);
            if (logEvent.Level == LogLevel.Error) {
                Debug.LogError(logEvent.Message);
            }
            else {
                Debug.Log(logEvent.Message);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent) {
            base.Write(logEvent);
            if (logEvent.LogEvent.Level == LogLevel.Error) {
                Debug.LogError(logEvent.LogEvent.Message);
            }
            else {
                Debug.Log(logEvent.LogEvent.Message);
            }
        }

        protected override void Write(IList<AsyncLogEventInfo> logEvents) {
            base.Write(logEvents);
            foreach (AsyncLogEventInfo asyncLogEventInfo in logEvents) {
                if (asyncLogEventInfo.LogEvent.Level == LogLevel.Error) {
                    Debug.LogError(asyncLogEventInfo.LogEvent.Message);
                }
                else {
                    Debug.Log(asyncLogEventInfo.LogEvent.Message);
                }
            }
        }
    }
}
