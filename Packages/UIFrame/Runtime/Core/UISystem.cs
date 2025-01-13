using AppFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
namespace UIFrame.Core {
    public class UISystem : ISystem {

        public void Awake() {
            _presenters = new List<IPresenter<IView, IModel>>();
        }
        public void Destroy() {
            
            UIRootRes.AssetHandle.Release();
            if (UIRootRes.GO != null) {
                Object.Destroy(UIRootRes.GO);
            }
            UIRootRes = default;
            
            foreach (IPresenter<IView, IModel> presenter in _presenters) {
                presenter.Destroy();
            }
        }
        public IEnumerator Start() {
            AssetHandle handle = _assetProvider.LoadAssetAsync("UIRoot");
            yield return handle;
            if (handle.Status == YooAsset.EOperationStatus.Succeed) {
                GameObject go = Object.Instantiate(handle.AssetObject) as GameObject;
                Object.DontDestroyOnLoad(go);
                UIRootRes = new HandleRes() {
                    AssetHandle = handle,
                    GO = go
                };
            }
        }

        public struct CrateParam {
            public IAssetProvider _assetProvider;
        }

        public struct HandleRes {
            public AssetHandle AssetHandle;
            public GameObject GO;
        }
        
        IAssetProvider _assetProvider;
        List<IPresenter<IView, IModel>> _presenters;
        public HandleRes UIRootRes;
        
        public UISystem(in UISystem.CrateParam createParam) {
            _assetProvider = createParam._assetProvider;
        }

        public void AddToRoot(IView view) {
            view.RootTransform.SetParent(UIRootRes.GO.transform);
        }

        public void RegisterPresenter(IPresenter<IView, IModel> presenter) {
            _presenters.Add(presenter);
        }

        public void UnRegisterPresenter(IPresenter<IView, IModel> presenter) {
            _presenters.Remove(presenter);
        }
        
        public void Update(float deltaTime) {
            foreach (IPresenter<IView,IModel> presenter in _presenters) {
                presenter.Update(deltaTime);
            }
        }
    }
}
