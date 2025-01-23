using AppFrame;
using System.Collections;
using IngameDebugConsole;
using UIDocument.Script.EventService;

namespace UIDocument.Script.DebugSystem {
    
    public class DebugSystem : ISystem {

        public void Update(float deltaTime) {
            // throw new System.NotImplementedException();
        }
        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Destroy() {
            // throw new System.NotImplementedException();
        }
        public IEnumerator Start() {
            // throw new System.NotImplementedException();
            DebugLogConsole.AddCommand("dump_round", "dump round", () => {
                _context.EventServiceProvider.GetEventService().TriggerEvent(this, "dump_round", new DumpRoundSystem());
            });
            yield return null;
        }
        
        public struct CreateParam {
            public DebugSystemContext Context;
        }

        DebugSystemContext _context;

        public struct DebugSystemContext : ISystemContext {
            public EventService.EventServiceProvider EventServiceProvider;
        }

        public DebugSystem(in CreateParam param) {
            _context = param.Context;   
        }
    }
}
