using AppFrame;
using UIDocument.Script.Core.Config;
namespace UIDocument.Script.Core.ADT {
    public struct BattleComponent : IComponent {
        
        public int dmg;
        public int energy;
        
        public static BattleComponent FromConfig(in BattleComponentConfig config) {
            return new BattleComponent() {
                dmg = config.dmg,
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
