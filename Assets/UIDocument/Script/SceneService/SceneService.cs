

using AppFrame;
using System;
using System.Collections;
using YooAsset;

namespace UIDocument.Script.SceneService {
    public class SceneService : IService {
        
        public const string SceneLoading = "SceneLoading";
        
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
            return YooAssets.LoadSceneAsync(sceneName);
        }
    }
}
