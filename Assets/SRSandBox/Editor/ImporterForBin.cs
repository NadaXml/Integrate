using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace SRSandBox.Editor {
    [ScriptedImporter(1, "bin")]
    public class ImporterForBin : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx) { 
            var s = File.ReadAllBytes(ctx.assetPath);
            var so = ScriptableObject.CreateInstance<FlatBufferBinary>();
            so.bytes = s;
            ctx.AddObjectToAsset("FlatBufferBinary", so);
            ctx.SetMainObject(so);
        }
    }
}
