

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
        
        public IEnumerator LoadSceneAsync(IServiceContext context, string sceneName) {
            var assetHandle = YooAssets.LoadSceneAsync(sceneName);
            context.SetHandleObject(SceneLoading, assetHandle);
            yield return assetHandle;
        }
    }
}
