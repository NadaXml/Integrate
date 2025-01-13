using Cysharp.Threading.Tasks;
using TMPro;
using UIFrame.Core;
using UnityEngine;
namespace UIDocument.Script.Module {
    public class LoadingView : IView {
        StartUp.Context _context;
        public LoadingView(StartUp.Context context) {
            _context = context;
        }

        public string RootAssetName;
        UISystem.HandleRes _RootHandle;
        
        public void Awake() {
            
        }
        public void Destroy() {
        }
        public Transform RootTransform
        {
            get {
                return _RootHandle.GO.transform;
            }
        }

        async UniTask PrepareAsset() {
            if (_RootHandle.GO == null) {
                var assetHandle = _context.AssetService.LoadAssetAsync(RootAssetName);
                await assetHandle.ToUniTask();
                _RootHandle = new UISystem.HandleRes() {
                    AssetHandle = assetHandle,
                    GO = Object.Instantiate(assetHandle.AssetObject) as GameObject,
                };
                _context.UISystem.AddToRoot(this);
            }
        }
        
        public async UniTaskVoid Render() { 
            await PrepareAsset();
            RenderProgress(0f);
        }

        public void RenderProgress(float progress) {
            
        }
    }
}
