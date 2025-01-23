using AppFrame;
using System;
namespace UIDocument.Script.RoundSystem.ADT {
    [Serializable]
    public struct ActionValue : IDumpable, IComparable<ActionValue>, IEquatable<ActionValue> {
        public bool Equals(ActionValue other) {
            return value == other.value;
        }
        public override bool Equals(object obj) {
            return obj is ActionValue other && Equals(other);
        }
        public override int GetHashCode() {
            return value;
        }

        /// <summary>
        /// 行动值：放大100倍的周期
        /// </summary>
        public int value;
        
        /// <summary>
        /// 通过速度得到最大行动值
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static ActionValue FromSpeed(in Speed speed) {
            return new ActionValue() {
                value = 1 / speed.value
            };
        }

        public static ActionValue ZERO = ActionValue.FromValue(0);
        
        /// <summary>
        /// 行动值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ActionValue FromValue(int value) {
            return new ActionValue() {
                value = value
            };
        }
        
        public static ActionValue operator +(ActionValue left, int right) {
            return new ActionValue() {
                value = left.value + right
            };
        }
        
        public static ActionValue operator +(ActionValue left, ActionValue right) {
            return new ActionValue() {
                value = left.value + right.value
            };
        }
        
        public static ActionValue operator -(ActionValue left, int right) {
            return new ActionValue() {
                value = left.value - right
            };
        }

        public static ActionValue operator -(ActionValue left, ActionValue right) {
            return new ActionValue() {
                value = left.value - right.value
            };
        }
        
        public static bool operator ==(ActionValue left, ActionValue right) {
            return left.value == right.value;
        }
        public static bool operator !=(ActionValue left, ActionValue right) {
            return !(left == right);
        }
        
        public static bool operator<(ActionValue left, ActionValue right) {
            return left.value < right.value;
        }
        public static bool operator >(ActionValue left, ActionValue right) {
            return left.value > right.value;
        }
        
        public static bool operator >= (ActionValue left, ActionValue right) {
            return left.value >= right.value;
        }
        public static bool operator <=(ActionValue left, ActionValue right) {
            return left.value <= right.value;
        }

        public bool IsPass() {
            return value <= 0;
        }
        public string Dump() {
            return value.ToString();
        }
        public int CompareTo(ActionValue other) {
            return value - other.value;
        }
    }
}
