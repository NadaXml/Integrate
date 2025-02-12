using UIDocument.Script.EventService;
using UnityEngine.Assertions;
namespace UIDocument.Script.Core.ADT {
    
    public class Actor {

        public struct ActorContext {
            public EventServiceProvider eventServiceProvider;
        }

        public ulong sequenceId;
        public ulong battleComponentRef;
        public ulong moveComponentRef;
        
        public ActorContext context { get; set; }
        ulong _componentId;

        public Actor(ActorContext context) {
            this.context = context;
        }

        public ulong AllocateComponentId() {
            #if DEBUG
                Assert.IsTrue(_componentId < ulong.MaxValue);
            #endif
            return _componentId++;
        }
    }
}
