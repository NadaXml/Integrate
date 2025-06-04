using System.Collections.Generic;
namespace extension {
    public static class AttrExtension {
        public static object[] ToDumpList(this cfg.Attr attr) {
            object[] data = new object[] {
                new {kk = "VBase", vv = attr.VBase},
                new {kk = "VAffect", vv = attr.VAffect},
                new {kk = "PAffect", vv = attr.PAffect},
                new {kk = "VFinal", vv = attr.VFinal},
                new {kk = "Dirty", vv = attr.Dirty}
            };
            return data;
        }
        
        public static string Dump(this cfg.Attr role) {
            return string.Empty;
        }
    }
}
