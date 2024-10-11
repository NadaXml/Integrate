
// ------ auto gen begin ------

namespace RedTipPart.Config {
    public class RedTipConstImp : RedTipHelper.Config.RedTipConst{
        public const string IslandHero = "RedTipIslandHero";
        public const string IslandHeroDict = "RedTipIslandHeroDict";
        public const string IslandHeroRef = "RedTipIslandHeroRef";
        public const string Main = "RedTipMain";

        string[] _keys;
        public override string[] GetKeys() {
            if (_keys == null) {
                _keys = new string[] {
                      IslandHero,
                      IslandHeroDict,
                      IslandHeroRef,
                      Main,
                };
            }
            return _keys;
        }
    }
}

// ------ auto gen end ------
