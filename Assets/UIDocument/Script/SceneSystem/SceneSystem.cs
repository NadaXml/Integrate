using AppFrame;
using System;
using System.Collections;
using UIDocument.Script.EventService;
using YooAsset;
namespace UIDocument.Script.SceneSystem {
    public class SceneSystem : ISystem {

        public struct CreateParameters {
            public AssetService.AssetService AssetService;
        }
        
        AssetService.AssetService _assetService;

        public SceneSystem(in CreateParameters createParameters) {
            _assetService = createParameters.AssetService;
            
        }

        public void Update(float deltaTime) {
            // throw new System.NotImplementedException();
        }
        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Destroy() {
            // throw new System.NotImplementedException();
        }
        public IEnumerator Start() {
            // throw new System.NotImplementedException();
            yield return null;
        }

        public SceneHandle LoadSceneAsync(string sceneName) {
            return _assetService.LoadSceneAsync(sceneName);
        }
    }
}
