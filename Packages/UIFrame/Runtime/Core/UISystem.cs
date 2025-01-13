using AppFrame;
using System.Collections;
using System.Collections.Generic;
namespace UIFrame.Core {
    public class UISystem : ISystem {

        public void Awake() {
            _presenters = new List<IPresenter<IView, IModel>>();
        }
        public void Destroy() {
            throw new System.NotImplementedException();
        }
        public IEnumerator Start() {
            throw new System.NotImplementedException();
        }

        public struct CrateParam {
            public IAssetProvider _assetProvider;
        }

        IAssetProvider _assetProvider;
        
        List<IPresenter<IView, IModel>> _presenters;
        
        public UISystem(in UISystem.CrateParam createParam) {
            _assetProvider = createParam._assetProvider;
        }

        public void RegisterPresenter(IPresenter<IView, IModel> presenter) {
            _presenters.Add(presenter);
        }
        public void Update(float deltaTime) {
            foreach (IPresenter<IView,IModel> presenter in _presenters) {
                presenter.Update(deltaTime);
            }
        }
    }
}
