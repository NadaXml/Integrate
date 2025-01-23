using Cysharp.Threading.Tasks;
using TMPro;
using UIFrame.Core;
using UIFrame.Core.UIComponent;
using UnityEngine;
namespace UIDocument.Script.Module {
    public class LoadingView : IView {
        StartUp.Context _context;
        public LoadingView(StartUp.Context context) {
            _context = context;
        }

        public string RootAssetName = "Loading";
        UISystem.HandleRes _RootHandle;
        UITree _RootTree;

        public struct UIComponentTemplate {
            public Slider LoadingProgress;
        }
        
        public void Awake() {
            
        }
        
        public void Destroy() {
            DestroyTemplate();
            GameObject.Destroy(_RootHandle.GO);
            _RootHandle.AssetHandle.Release();
        }

        UIComponentTemplate _componentTemplate;
        public Transform RootTransform
        {
            get {
                return _RootHandle.GO.transform;
            }
        }
        public bool IsRootOK
        {
            get {
                return _RootHandle.AssetHandle is {IsValid: true, IsDone: true};
            }
        }

        public void BindTemplate(UITree template) {
            Slider LoadingProgress = null;
            // 可能需要排序，因为嵌套组件后置比较好
            for (int i = 0; i < template.keys.Count; i++) {
                var name = template.keys[i];
                var uiComponent = template.sliders[i];
                if (name == "LoadingProgress") {
                    LoadingProgress = uiComponent;
                }
                uiComponent.Awake();
            }
            // 需要模拟所有权转移的情况
            template.keys.Clear();
            template.sliders.Clear();
            _componentTemplate = new UIComponentTemplate() {
                LoadingProgress = LoadingProgress,
            };
            
        }
        
        public void DestroyTemplate() {
            _componentTemplate.LoadingProgress?.Destroy();
        }

        async UniTask PrepareAsset() {
            if (_RootHandle.GO == null) {
                var assetHandle = _context.AssetService.LoadAssetAsync<GameObject>(RootAssetName);
                await assetHandle.ToUniTask();
                _RootHandle = new UISystem.HandleRes() {
                    AssetHandle = assetHandle,
                    GO = (GameObject)Object.Instantiate(assetHandle.AssetObject),
                };
                _context.UISystem.AddToRoot(this);
                _RootTree = _RootHandle.GO.GetComponent<UITree>();
                BindTemplate(_RootTree);
            }
        }
        
        public async void Render() { 
            await PrepareAsset();
            RenderProgress(0f);
        }

        public void RenderProgress(float progress) {
            _componentTemplate.LoadingProgress.SetProgress(progress);
        }
    }
}
