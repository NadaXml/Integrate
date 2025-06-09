using System;
using UnityEditor.SceneManagement;
namespace adt {
    public struct RoundActionValue : IEquatable<RoundActionValue> {
        public ActionValue actionValue;
        public ActionValue maxActionValue;

        public static RoundActionValue FromSpeed(in Attr speed) {
            return new RoundActionValue() {
                actionValue = ActionValue.FromSpeed(speed),
                maxActionValue = ActionValue.FromSpeed(speed)
            };
        }

        public void Forward() {
            actionValue -= 1;
        }

        public bool IsPass() {
            return actionValue.IsPass();
        }

        public void Reset() {
            actionValue = maxActionValue;
        }

        public string Dump() {
            return $"value is {actionValue.Dump()}\n max is {maxActionValue.Dump()}";
        }
        public bool Equals(RoundActionValue other) {
            return actionValue.Equals(other.actionValue) && maxActionValue.Equals(other.maxActionValue);
        }
        public override bool Equals(object obj) {
            return obj is RoundActionValue other && Equals(other);
        }
        public override int GetHashCode() {
            return HashCode.Combine(actionValue, maxActionValue);
        }

        /// <summary>
        /// 速度变化，修改最大行动值，适应当前行动值变化
        /// </summary>
        /// <param name="speed"></param>
        public void AdjustActionValueOnSpeedChange(in Attr speed) {
            var prevMaxActionValue = maxActionValue;
            maxActionValue = ActionValue.FromSpeed(speed);
            actionValue = actionValue / prevMaxActionValue * maxActionValue;
        }
        
        public void AdvanceActionValueP(int p) {  // 是否立即结算？对结果有区别
            var v = p * 0.0001f * maxActionValue;
            if (actionValue > v) {
                actionValue -= v;
            }
            else {
                actionValue = ActionValue.Zero;
            }
        }

        public void DelayActionValueP(int p) { // 是否立即结算？对结果有区别
            var v = p * 0.0001f * maxActionValue;
            actionValue += v;
        }
    }
}
