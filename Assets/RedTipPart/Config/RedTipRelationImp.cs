using System.Collections.Generic;
namespace RedTipPart.Config {
    public class RedTipRelationImp : RedTipHelper.Config.RedTipRelation {

        Dictionary<string, List<string>> _relation;
        public override Dictionary<string, List<string>> Relation
        {
            get {
                if (_relation == null) {
                    _relation = new Dictionary<string, List<string>>() {
                        {
                            RedTipConstImp.Main, 
                            new List<string> {
                                RedTipConstImp.IslandHero,
                                RedTipConstImp.IslandHeroRef
                            }
                        },
                        {
                            RedTipConstImp.IslandHero, 
                            new List<string> {
                                RedTipConstImp.IslandHeroDict,
                            }
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
                        {
                            RedTipConstImp.IslandHeroRef, 
                            new List<string> {
                                RedTipConstImp.IslandHero
                            }
                        }
                    };
                }
                return _refRelation;
            }
        }
    }
}
