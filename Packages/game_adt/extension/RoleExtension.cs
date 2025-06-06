using cfg;
using Scriban;
namespace extension {
    public static class RoleExtension {

        public static object[] ToDumpList(this role role) {
            object[] data = new object[] {
                new { kk = "Id", vv = role.Id},
                new { kk = "EquipId", vv = role.EquipId},
                new { kk = "AttrGroup", vv = role.AttrGroup?.ToDumpList()},
            };
            return data;
        }
        
        public static string Dump(this role role) {
            var template = Template.Parse(TemplateDefine.ObjectKeyValueTemplate);
            var result = template.Render(new {model = role.ToDumpList()});
            return result;
        }
    }
}
