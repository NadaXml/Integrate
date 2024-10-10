namespace RedTipDebug.Editor {

    public class RedTipTreeElement : TreeElement {
        public bool RedTipActive;
        public float TestFloat1;
        public bool enabled;

        public RedTipTreeElement(string name, bool active, int depth, int id)
            : base(name, depth, id) {

            enabled = true;
            RedTipActive = active;
            TestFloat1 = 0f;
        }
    }
}
