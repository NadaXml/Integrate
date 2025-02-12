using AppFrame;
using UIDocument.Script.Core.Config;
namespace UIDocument.Script.Core.ADT {
    public struct BattleComponent : IComponent {
        
        public Attribute atk;
        public Attribute def;
        public Attribute hp;
        
        public int energy;
        
        public static BattleComponent FromConfig(in BattleComponentConfig config) {
            return new BattleComponent() {
                atk = new Attribute(config.atk),
                def = new Attribute(config.def),
                hp = new Attribute(config.hp),
                energy = config.energy
            };
        }
        public ulong actorSequenceId
        {
            get;
            set;
        }
        public ulong sequenceId
        {
            get;
            set;
        }
    }
}
