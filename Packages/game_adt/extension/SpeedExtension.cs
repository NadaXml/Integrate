using cfg;
using Scriban;
namespace extension {
    public static class SpeedExtension {
        public static object[] ToDumpList(this cfg.Speed speed) {
            object[] data = new object[] {
                new {kk = "V", vv = speed.V?.ToDumpList()}
            };
            return data;
        }
        
        public static string Dump(this cfg.Speed speed) {
            var template = Template.Parse(TemplateDefine.ObjectKeyValueTemplate);
            var result = template.Render(new {model = speed.ToDumpList()});
            return result;
        }
    }
}
