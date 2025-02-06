using System;
using System.Text;
namespace UIDocument.Script.EventService {
    
    // 事件定义需要走代码生成

    public abstract class EventNameDef {
        public const string DumpRound = "dump_round";
        public const string DumpRoundInspector = "dump_round_inspector";
    }
    
    
    public class DumpRoundSystem : GameEventBase {
        
        public DumpRoundSystem() {
            EventId = 1;
        }
    }
    
    public class FetchRoundInspector : DumpRoundSystem {

        // 可以改成字节流
        public string Result;
        
        public FetchRoundInspector() {
            EventId = 2;
        }
    }
}
