using System;
namespace adt {
    public struct Attr : IEquatable<Attr> {
        public int valueBase;
        public int valueFinal;
        public int valueAffect;
        public int percentAffect;
        public bool dirty;

        public Attr(int value) {
            valueBase = value;
            valueFinal = valueBase;
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

        public void Apply() {
            if (dirty) {
                valueFinal = (int)(valueBase * (1 * percentAffect * 0.0001f)) + valueAffect;
            }
        }

        public string Dump() {
            return $"{{ valueBase = {valueBase}, valueFinal = {valueFinal}, valueAffect = {valueAffect} percentAffect = {percentAffect} dirty = {dirty} }}";
        }
        public bool Equals(Attr other) {
            return valueFinal == other.valueFinal;
        }
        public override bool Equals(object obj) {
            return obj is Attr other && Equals(other);
        }
        public override int GetHashCode() {
            return valueFinal.GetHashCode();
        }
    }
}
