
using System.Collections.Generic;
namespace RedTipHelper.Config {
    
    public abstract class RedTipConst {
        public abstract string Root { get; }

        public abstract string[] GetKeys();
    }

    public abstract class RedTipRelation {
        public virtual Dictionary<string, List<string>> Relation { get; }
        public virtual Dictionary<string, List<string>> RefRelation { get; }
    }
}
