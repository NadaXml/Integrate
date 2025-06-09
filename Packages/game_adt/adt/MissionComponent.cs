using cfg;
namespace adt {
    public struct MissionComponent {
        public readonly cfg.mission mission;

        public MissionComponent(in cfg.mission mission) {
            this.mission = mission;
        }
    }
}
