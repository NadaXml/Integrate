using System.Collections.Generic;
namespace RedTipPart.Config {
    public class RedTipRelation : RedTipHelper.Config.RedTipRelation {

        Dictionary<string, List<string>> _relation;
        public override Dictionary<string, List<string>> Relation
        {
            get {
                if (_relation == null) {
                    _relation = new Dictionary<string, List<string>>() {
                        [RedTipConst.Main] = {
                            RedTipConst.IslandHero,
                            RedTipConst.IslandHeroRef
                        },
                        [RedTipConst.IslandHero] = {
                            RedTipConst.IslandHeroDict
                        }
                    };
                }
                return _relation;
            }
        }

        Dictionary<string, List<string>> _refRelation;
        public override Dictionary<string, List<string>> RefRelation
        {
            get {
                if (_refRelation == null) {
                    _refRelation = new Dictionary<string, List<string>>() {
                        [RedTipConst.IslandHeroRef] = {
                            RedTipConst.IslandHero
                        }
                    };
                }
                return _refRelation;
            }
        }
    }
}
