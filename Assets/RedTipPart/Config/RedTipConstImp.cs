
// ------ auto gen begin ------

using RedTipHelper.Core;
using RedTipPart.Part;

namespace RedTipPart.Config {
    public class RedTipConstImp : RedTipHelper.Config.RedTipConst{
        public const string IslandHero = "RedTipIslandHero";
        public const string IslandHeroDict = "RedTipIslandHeroDict";
        public const string IslandHeroRef = "RedTipIslandHeroRef";
        public const string Main = "RedTipMain";
        
        // 这里强制
        public override string Root
        {
            get;set;
        }

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

        public void BindCreator(IRedTipService service) {
            RedTipService.BindCreate(RedTipConstImp.IslandHero, RedTipIslandHeroCreator);
            RedTipService.BindCreate(RedTipConstImp.IslandHeroDict, RedTipIslandHeroDictCreator);
            RedTipService.BindCreate(RedTipConstImp.IslandHeroRef, RedTipIslandHeroRefCreator);
            RedTipService.BindCreate(RedTipConstImp.Main, RedTipMainCreator);
        }

        static RedTipBase RedTipIslandHeroCreator(string key, IRedTipContext service) { return new RedTipIslandHero(key, service); }
        static RedTipBase RedTipIslandHeroDictCreator(string key, IRedTipContext service) { return new RedTipIslandHeroDict(key, service); }
        static RedTipBase RedTipIslandHeroRefCreator(string key, IRedTipContext service) { return new RedTipIslandHeroRef(key, service); }
        static RedTipBase RedTipMainCreator(string key, IRedTipContext service) { return new RedTipMain(key, service); }
    }
}

// ------ auto gen end ------
