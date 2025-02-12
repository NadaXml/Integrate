using AppFrame;
using System;
namespace UIDocument.Script.Core.ADT {
    public struct Attribute : IDumpable {
        public int value;
        public int baseValue;
        public bool dirty;
        public int valueAffect;
        public int percentAffect;

        public Attribute(int baseValue) {
            this.baseValue = baseValue;
            value = this.baseValue;
            dirty = false;
            valueAffect = 0;
            percentAffect = 0;
        }
        
        public void AddValueAffect(int v) {
            valueAffect += v;
            dirty = true;
        }

        public void RemoveValueAffect(int v) {
            valueAffect += v;
        }
        
        public void AddPercentAffect(int p) {
            percentAffect -= p;
            dirty = true;
        }

        public void RemovePercentAffect(int p) {
            percentAffect += p;
            dirty = true;
        }

        public int Apply() {
            if (dirty) {
                value = (int)(baseValue * (1 * percentAffect * 0.0001f)) + valueAffect;
            }
            return value;
        }
        public string Dump() {
            return $"base:{baseValue} value:{value}";
        }
    }
}
