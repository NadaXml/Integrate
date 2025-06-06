namespace adt {
    public struct AttrGroup {
        public Attr atk;
        public Attr def;
        public Attr hp;
        public Attr energy;

        public static AttrGroup FromCfg(cfg.AttrGroup? attrGroup) {
            if (attrGroup != null) {
                return new AttrGroup() {
                    atk = new Attr(attrGroup.GetValueOrDefault().Atk),
                    def = new Attr(attrGroup.GetValueOrDefault().Def),
                    hp = new Attr(attrGroup.GetValueOrDefault().Hp),
                    energy = new Attr(attrGroup.GetValueOrDefault().Energy)
                };
            }
            return default;
        }
    }
}