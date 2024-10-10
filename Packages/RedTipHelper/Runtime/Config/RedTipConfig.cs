using Core;
using System.Collections.Generic;
namespace Config {
    // TODO 代码生成
    public abstract class RedTipConst {
        public const string Main = "Main";
        public const string IslandHero = "IslandHero";
        public const string IslandHero2 = "IslandHero";
        public const string IslandHeroLifeCareer = "IslandHeroLifeCareer";
        public const string IslandHeroAdventure = "IslandHeroAdventure";

        public static string[] Keys = {
            Main,
            IslandHero,
            IslandHero2,
            IslandHeroLifeCareer,
            IslandHeroAdventure,
        };
    }

    public abstract class RedTipRelation {
        public static Dictionary<string, List<string>> Relation = new Dictionary<string, List<string>>() {
            [RedTipConst.Main] = {RedTipConst.IslandHero},
            [RedTipConst.IslandHero] = {
                RedTipConst.IslandHeroLifeCareer,
                RedTipConst.IslandHeroAdventure
            }
        };

        public static Dictionary<string, List<string>> RefRelation = new Dictionary<string, List<string>>() {
            [RedTipConst.IslandHero2] = {RedTipConst.IslandHero} 
        };
    }
}
