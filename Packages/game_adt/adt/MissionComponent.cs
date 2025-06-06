using cfg;
namespace adt {
    public struct MissionComponent {
        public readonly Tbmission tbmission;

        public MissionComponent(ref Tbmission tbmission) {
            this.tbmission = tbmission;
        }
    }
}
