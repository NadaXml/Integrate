using System.Collections.Generic;
namespace extension {
    public static class SpeedExtension {
        public static object[] ToDumpList(this cfg.Speed speed) {
            object[] data = new object[] {
                new {kk = "V", vv = speed.V?.ToDumpList()}
            };
            return data;
        }
        
        public static string Dump(this cfg.Speed speed) {
            return string.Empty;
        }
    }
}
