using RedTipHelper.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriban;
using Scriban.Runtime;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

namespace RedTipCodeGenerator {

    public class ScribanHelper
    {
        public static string ConfigOutputDir = "Assets/RedTipPart/Config";
        public static string ConfigFileName = "RedTipConstImp";
        public static string ConfigAssemblyName = "Assembly-CSharp";
        
        const string TemplateSrc =
@"
// ------ auto gen begin ------

namespace RedTipPart.Config {
    public class RedTipConst : RedTipHelper.Config.RedTipConst{
{{~ for $obj in sp ~}}
        public const string {{ $obj.name }} = ""{{ $obj.path }}"";
{{~ end ~}}

        string[] _keys;
        public override string[] GetKeys() {
            if (_keys == null) {
                _keys = new string[] {
                    {{~ for $obj in sp ~}}
                      {{ $obj.name }},
                    {{~ end ~}}
                };
            }
            return _keys;
        }
    }
}

// ------ auto gen end ------
";
        [MenuItem("RedTipCode/GenerateConfig")]
        public static void ConfigTemplate() {
            
            if (string.IsNullOrEmpty(ConfigOutputDir)) {
                Debug.LogError("没有配置导出路径");
                return;
            }

            if (EditorApplication.isCompiling) {
                return;
            }
            
            AssetDatabase.Refresh();

            string[] findAssets = AssetDatabase.FindAssets(ConfigFileName, new string[] {
                ConfigOutputDir
            });

            if (findAssets.Length > 0) {
                foreach (string findAsset in findAssets) {
                    AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(findAsset));
                }
            }
            
            AssetDatabase.Refresh();
            
            string txt = GenerateConfig();
            string fullPath = Path.Combine(Application.dataPath, $"{ConfigOutputDir.Replace("Assets/", "")}/{ConfigFileName}.cs");
            File.WriteAllText(fullPath, txt);
            Debug.Log("write fullPath " + fullPath + "\n" + txt);
        }

        static string GenerateConfig() {
            
            Assembly assembly = Assembly.Load(ConfigAssemblyName);
            var iter = assembly.GetTypes().Where(t => {
                return t.Namespace == "RedTipPart.Part";
            });
            
            var array = new ScriptArray();
            foreach (Type type in iter) {
                var pairObject = new ScriptObject();
                pairObject.Add("path", type.Name);
                pairObject.Add("name", type.Name.Replace("RedTip",""));
                array.Add(pairObject);
            }
            
            var scriptObject = new ScriptObject();
            scriptObject.Add("sp", array);

            Template t = Template.Parse(TemplateSrc);
            string ret = t.Render(scriptObject);
            return ret;
        }
    }

    public class RedTipRRGen : EditorWindow {

        public string name;

        public RedTipCalc.CalcType calcType;

        public static string OutputDir = "Assets/RedTipPart/Part";
        
        const string TemplateBaseSrc =
@"
using RedTipHelper.Core;

namespace RedTipPart.Part {
    public class {{ name }} : RedTipBase {

        public {{ name }}(string key, IRedTipContext context) : base(key, context) {
{{ if calcType == 1 }} ValCalc = new RedTipCalcDefault(context, this, CalcImp); {{ end }}
{{ if calcType == 2 }} ValCalc = new RedTipCalcDict<int>(context, this, CalcImp); {{ end }}
{{ if calcType == 3 }} var calc = new RedTipCalcRefOther(context, this);
            var keys = GetRefKeys();
            calc.SetRef(keys);
            ValCalc = calc; {{ end }}
        }

        protected override void OnAwake() {
            
        }
        
        protected override void OnStart() {
{{ if calcType == 1 }} // RedTipCalcDefault calc = ValCalc as RedTipCalcDefault; {{ end }}
{{ if calcType == 2 }} // RedTipCalcDict<int> calc = ValCalc as RedTipCalcDict<int>; {{ end }}
{{ if calcType == 3 }} // RedTipCalcRefOther calc = ValCalc as RedTipCalcRefOther;{{ end }}
        }

        protected override void OnCalc() {
          
       }

        protected override void OnDestroy() {
  
        }

{{ if calcType == 1 }} bool CalcImp() {
            return false;
        } {{ end }}
{{ if calcType == 2 }} bool CalcImp(int key) {
            return false;
        } {{ end }}
    }
}
";

        [MenuItem("RedTipCode/ScriptGenerate")]
        public static void RedTipRRBaseTemplateDict() {

            var appearanceEditor = GetWindow<RedTipRRGen>(false, "RedTipRRBaseGenerator", true);
            appearanceEditor.Show();
        }

        void OnGUI() {
            EditorGUILayout.LabelField($"生成路径：{OutputDir}");
            EditorGUILayout.LabelField("请输入类名(等于文件名)：");
            name = EditorGUILayout.TextField(name);
            calcType = (RedTipCalc.CalcType)EditorGUILayout.EnumPopup(calcType);

            if (GUILayout.Button("生成")) {
                GenRRFile(name, calcType);
            }
        }

        void GenRRFile(string name, RedTipCalc.CalcType calcType) {
            if (string.IsNullOrEmpty(OutputDir)) {
                Debug.LogError("没有配置路径");
                return;
            }
            string filePath = $"{OutputDir}/{name}.cs";

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
