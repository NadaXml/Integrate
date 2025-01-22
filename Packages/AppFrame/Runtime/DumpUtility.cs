using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AppFrame {

    public interface IDumpable {
        string Dump();
    } 
    
    public static class DumpUtility {
        public static string DumpList<T>(IEnumerable<T> list) where T : IDumpable {
            if (list == null) {
                return "null";
            }

            bool isEmpty = true;
            StringBuilder builder = new StringBuilder();
            foreach (T dump in list) {
                builder.AppendLine(dump.Dump());
                isEmpty = false;
            }
            if (isEmpty) {
                return "empty[]";
            }
            else {
                return builder.ToString();
            }
        }
    }
}
