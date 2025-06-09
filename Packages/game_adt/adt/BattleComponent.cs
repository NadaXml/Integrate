namespace adt {
    public struct BattleComponent {
        public AttrGroup attrGroup;

        public static BattleComponent FromCfg(cfg.role role) {
            var battle = new BattleComponent() {
                attrGroup = adt.AttrGroup.FromCfg(role.AttrGroup)
            };
            return battle;
        }
    }
}