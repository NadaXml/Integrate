using AppFrame;
using System;
namespace UIDocument.Script.RoundSystem.ADT {
    [Serializable]
    public struct Speed : IDumpable, IEquatable<Speed> {
        /// <summary>
        /// 速度：放大100倍的频率
        /// </summary>
        public int value;
        
        public static Speed FromValue(int value) {
            return new Speed {
                value = value
            };
        }
        public string Dump() {
            return value.ToString();
        }
        public bool Equals(Speed other) {
            return value == other.value;
        }
        public override bool Equals(object obj) {
            return obj is Speed other && Equals(other);
        }
        public override int GetHashCode() {
            return value;
        }
    }
}
