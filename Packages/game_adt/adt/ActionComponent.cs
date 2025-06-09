using System;

namespace adt {
    public struct ActionComponent : IComparable<ActionComponent>, IEquatable<ActionComponent> {
        public Attr speed;
        public int position;
        public RoundActionValue roundActionValue;

        public static ActionComponent FromCfg(cfg.role role, cfg.setting setting) {
            var action = new ActionComponent(role.AttrGroup.GetValueOrDefault().Speed, setting.Index);
            return action;
        }

        public ActionComponent(int speed, int position) {
            this.speed = new Attr(speed);
            this.position = position;
            roundActionValue = RoundActionValue.FromSpeed(this.speed);
        }

        public string Dump() {
            return $"{{ {roundActionValue.Dump()}\n speed is {speed.Dump()} position = {position} }}";
        }

        public int CompareTo(ActionComponent other) {
            var a = roundActionValue.actionValue.CompareTo(other.roundActionValue.actionValue);
            if (a == 0) {
                return position - other.position;
            }
            return a;
        }
        public bool Equals(ActionComponent other) {
            return position == other.position;
        }
        
        public override int GetHashCode() {
            return HashCode.Combine(speed, position, roundActionValue.GetHashCode());
        }

        public void ApplySpeedEffect() {
            var prevSpeed = speed;
            speed.Apply();
            if (!speed.Equals(prevSpeed)) {
                roundActionValue.AdjustActionValueOnSpeedChange(speed);
            }
        }
    }
}
