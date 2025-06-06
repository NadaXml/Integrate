using Scriban;
namespace extension {
    public static class AttrGroupExtension {
        public static object[] ToDumpList(this cfg.AttrGroup attrGroup) {
            object[] data = new object[] {
                new {kk = "atk", vv = attrGroup.Atk},
                new {kk = "def", vv = attrGroup.Def},
                new {kk = "hp", vv = attrGroup.Hp},
                new {kk = "energy", vv = attrGroup.Energy}
            };
            return data;
        }
        
        public static string Dump(this cfg.AttrGroup attr) {
            var template = Template.Parse(TemplateDefine.ObjectKeyValueTemplate);
            var result = template.Render(new {model = attr.ToDumpList()});
            return result;
        }
    }
}
