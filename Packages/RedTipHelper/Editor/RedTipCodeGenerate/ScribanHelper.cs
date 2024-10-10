using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriban;
using Scriban.Runtime;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace RedTipCodeGenerator {

    public class ScribanHelper
    {

        const string TemplateSrc =
@"RedTipRRKey = {
   {{~ for $obj in sp ~}}
      {{ $obj.name }} = ""{{ $obj.path }}"",
   {{~ end ~}}
}";
        [MenuItem("RedTipCode/ConfigTemplate")]
        public static void ConfigTemplate() {

            AssetDatabase.Refresh();

            string confgPath = "Assets/Lib/Scripts/LocalData/RedTipRR/RedTipRRConfig.txt";
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(confgPath);
            string replaceTxt = GenerateConfig();
            string finalTxt = ReplaceCode(asset.text, replaceTxt);
            string fullPath = AssetDatabase.GetAssetPath(asset);
            if (fullPath.StartsWith("Assets/")) {
                fullPath = fullPath.Replace("Assets/", "");
            } else if (fullPath.StartsWith("Assets\\")) {
                fullPath = fullPath.Replace("Assets\\", "");
            }
            fullPath = Path.Combine(Application.dataPath, fullPath);
            File.WriteAllText(fullPath, finalTxt);
            Debug.Log("write fullPath " + fullPath + "\n" + finalTxt);
        }

        static string GenerateConfig() {
            var root = "Assets/Lib/Scripts/LocalData/RedTipRR/Part";

            string[] assetguids = AssetDatabase.FindAssets("t:TextAsset", new string[]{root});

            var array = new ScriptArray();

            foreach (var s in assetguids) {
                string path = AssetDatabase.GUIDToAssetPath(s);
                var pairObject = new ScriptObject();
                string pp = path;
                if (pp.StartsWith("Assets/Lib/Scripts/")) {
                    pp = pp.Replace("Assets/Lib/Scripts/", "");
                }
                if (pp.EndsWith(".txt")) {
                    pp = pp.Replace(".txt", "");
                }
                pp = pp.Replace("/", ".");
                string name = Path.GetFileNameWithoutExtension(path);
                pairObject.Add("path", name);
                pairObject.Add("name", name.Replace("RedTipRR", ""));
                array.Add(pairObject);
            }

            var scriptObject = new ScriptObject();
            scriptObject.Add("sp", array);

            Template t = Template.Parse(TemplateSrc);
            string ret = t.Render(scriptObject);
            return ret;
        }

        static string KeyHeadTag = "------ auto gen head ------";
        static string KeyTailTag = "------ auto gen end ------";

        static string ReplaceCode(string sourceTxt, string replaceTxt) {
            string pattern = $@"{Regex.Escape(KeyHeadTag)}(.*?)\s{Regex.Escape(KeyTailTag)}";
            string replace = $"{KeyHeadTag}\n{replaceTxt}\n{KeyTailTag}";
            return Regex.Replace(sourceTxt, pattern, replace, RegexOptions.Singleline);
        }
    }

    public class RedTipRRGen : EditorWindow {

        public string name;
        public enum CalcType {
            None = 0,
            Default = 1,
            Dict = 2,
            RefOther = 3
        }
        public CalcType calcType;

        const string TemplateBaseSrc =
@"
--- @class {{ name }} : RedTipBaseRR
local {{ name }} = Class({}, Assets.req(""LocalData.RedTipRR.RedTipRRBase""))
local RedTipCalc = Assets.req(""LocalData.RedTipRR.RedTipRRCalc"")

function {{ name }}:onAwake()
{{  if calcType == 1 }}
    self.valCalc = RedTipCalc:createDefault(self, self.impCalcDefault)
{{ else if calcType == 2 }}
    self.valCalc = RedTipCalc:createDict(self, self.impCalcDict)
{{ else if calcType == 3 }}
    self.valCalc = RedTipCalc:createRefOther(self)
{{ else }}
{{ end }}
end

function {{ name }}:onDestroy()

end

function {{ name }}:onStart()
end

{{  if calcType == 1 }}
function {{ name }}:impCalcDefault()
end
{{ else if calcType == 2 }}
function {{ name }}:impCalcDict(key)
end
{{ else if calcType == 3 }}
function {{ name }}:impCalcRef(locations)
end
{{ else }}
{{ end }}

return {{ name }}
";

        [MenuItem("RedTipCode/RedTipRRBaseTemplateDict")]
        public static void RedTipRRBaseTemplateDict() {

            var appearanceEditor = GetWindow<RedTipRRGen>(false, "RedTipRRBaseGenerator", true);
            appearanceEditor.Show();
        }

        void OnGUI() {
            name = EditorGUILayout.TextField(name);
            calcType = (CalcType)EditorGUILayout.EnumPopup(calcType);

            if (GUILayout.Button("生成")) {
                GenRRFile(name, calcType);
            }
        }

        void GenRRFile(string name, CalcType calcType) {
            string filePath = $"Assets/Lib/Scripts/LocalData/RedTipRR/Part/{name}.txt";

            if (File.Exists(filePath)) {
                Debug.Log("文件已经存在，生成失败");
                return;
            }

            var scriptObject = new ScriptObject();
            scriptObject.Add("name", name);
            scriptObject.Add("calcType", (int)calcType);

            Template t = Template.Parse(TemplateBaseSrc);
            string finalText = t.Render(scriptObject);

            File.WriteAllText(filePath, finalText);
            Debug.Log("write fullPath " + filePath + "\n" + finalText);
        }
    }

}
