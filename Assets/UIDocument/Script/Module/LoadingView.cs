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

        public const string C_RootAssetName = "Loading";
        UISystem.HandleRes _rootHandle;
        UITree _rootTree;

        public struct UIComponentTemplate {
            public Slider loadingProgress;
        }
        
        public void Awake() {
            
        }
        
        public void Destroy() {
            DestroyTemplate();
            GameObject.Destroy(_rootHandle.GO);
            _rootHandle.AssetHandle.Release();
        }

        UIComponentTemplate _componentTemplate;
        public Transform RootTransform
        {
            get {
                return _rootHandle.GO.transform;
            }
        }
        public bool IsRootOK
        {
            get {
                return _rootHandle.AssetHandle is {IsValid: true, IsDone: true};
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
                loadingProgress = LoadingProgress,
            };
            
        }
        
        public void DestroyTemplate() {
            _componentTemplate.loadingProgress?.Destroy();
        }

        async UniTask PrepareAsset() {
            if (_rootHandle.GO == null) {
                var assetHandle = _context.assetService.LoadAssetAsync<GameObject>(C_RootAssetName);
                await assetHandle.ToUniTask();
                _rootHandle = new UISystem.HandleRes() {
                    AssetHandle = assetHandle,
                    GO = (GameObject)Object.Instantiate(assetHandle.AssetObject),
                };
                _context.uiSystem.AddToRoot(this);
                _rootTree = _rootHandle.GO.GetComponent<UITree>();
                BindTemplate(_rootTree);
            }
        }
        
        public async void Render() { 
            await PrepareAsset();
            RenderProgress(0f);
        }

        public void RenderProgress(float progress) {
            _componentTemplate.loadingProgress.SetProgress(progress);
        }
    }
}
