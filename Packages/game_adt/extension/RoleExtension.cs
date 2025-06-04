using cfg;
using Scriban;
using System.Collections.Generic;
namespace extension {
    public static class RoleExtension {

        public static object[] ToDumpList(this role role) {
            object[] data = new object[] {
                new { kk = "Id", vv = role.Id},
                new { kk = "EquipId", vv = role.EquipId},
                new { kk = "Speed", vv = role.Speed?.ToDumpList()},
                new { kk = "Atk", vv = role.Atk?.ToDumpList()},
                new { kk = "Def", vv = role.Def?.ToDumpList()},
                new { kk = "Hp", vv = role.Hp?.ToDumpList()},
            };
            return data;
        }

        public static string templateTxt = @"
{{func expandList }}
    {{ if $0 | object.typeof == ""array"" }}
        {{~ for kv in $0 ~}}
            {{ kv.kk }} = {{ expandList kv.vv }}
        {{~ end ~}}
    {{ else }}
        {{ $0 }}
    {{ end }}
{{ end }}



{{~ for kv in data ~}}
    {{ kv.kk }} = {{ expandList kv.vv }}
{{~ end ~}}
";
        
        public static string Dump(this role role) {
            var template = Template.Parse(templateTxt);
            var result = template.Render(new {data = role.ToDumpList()});
            return result;
        }
    }
}
