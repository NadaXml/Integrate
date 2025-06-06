namespace adt {
    public struct Attr {
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

        public int Apply() {
            if (dirty) {
                valueFinal = (int)(valueBase * (1 * percentAffect * 0.0001f)) + valueAffect;
            }
            return valueFinal;
        }
    }
}
