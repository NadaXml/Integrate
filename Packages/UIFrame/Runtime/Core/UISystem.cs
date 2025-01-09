using AppFrame;
using System.Collections;
namespace UIFrame.Core {
    public class UISystem : ISystem {

        public void Awake() {
            throw new System.NotImplementedException();
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

        public UISystem(in UISystem.CrateParam createParam) {
            _assetProvider = createParam._assetProvider;
        }
        
        
    }
}
