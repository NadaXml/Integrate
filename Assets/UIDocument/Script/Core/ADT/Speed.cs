using AppFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
namespace UIDocument.Script.Core.ADT {
    /// <summary>
    /// 速度：放大100倍的频率
    /// </summary>
    [Serializable]
    public struct Speed : IDumpable, IEquatable<Speed> {
        public Attribute attribute;
        
        public static Speed FromValue(int value) {
            return new Speed {
                attribute = new Attribute() {
                    baseValue = value,
                    value = value,
                    valueAffect = 0,
                    percentAffect = 0,
                    dirty = false
                }
            };
        }
        
        public bool Equals(Speed other) {
            return attribute.value == other.attribute.value;
        }
        
        public override int GetHashCode() {
            return attribute.value;
        }
        
        public string Dump() {
            return attribute.Dump();
        }
    }
}
