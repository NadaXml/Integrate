using AppFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
namespace UIDocument.Script.Core.ADT {
    [Serializable]
    public struct Speed : IDumpable, IEquatable<Speed> {

        public int baseValue;
        /// <summary>
        /// 速度：放大100倍的频率
        /// </summary>
        public int value;
        public bool dirty;
        
        public int valueAffect;
        public int percentAffect;

        public static Speed FromValue(int value) {
            return new Speed {
                baseValue = value,
                value = value,
                valueAffect = 0,
                percentAffect = 0,
                dirty = false
            };
        }
        public string Dump() {
            return $"base:{baseValue} value:{value}";
        }
        public bool Equals(Speed other) {
            return value == other.value;
        }
        
        public override int GetHashCode() {
            return value;
        }
        
        
        /// <summary>
        /// 加速，减速
        /// </summary>
        public void AddSpeedValueAffect(int v) {
            valueAffect += v;
            dirty = true;
        }

        public void RemoveSpeedValueAffect(int v) {
            valueAffect += v;
        }
        
        public void AddSpeedPercentAffect(int p) {
            percentAffect -= p;
            dirty = true;
        }

        public void RemoveSpeedPercentAffect(int p) {
            percentAffect += p;
            dirty = true;
        }

        public int Apply() {
            if (dirty) {
                value = (int)(baseValue * (1 * percentAffect * 0.0001f)) + valueAffect;
            }
            return value;
        }
    }
}
