using YooAsset;
using AppFrame;
using System.Collections;
using UIFrame.Core;
using UnityEngine;

namespace UIDocument.Script.AssetService {
    public class AssetService : IService, IAssetProvider {

        string _packageName;
        EPlayMode _playMode = EPlayMode.OfflinePlayMode;
        
        public AssetService(string packageName, EPlayMode playMode) {
            _packageName = packageName;
            _playMode = playMode;
        }
        
        public void Awake() {
        }
        public void Destroy() {
            
            var package = YooAssets.GetPackage(_packageName);
            package.ClearAllCacheFilesAsync();
            
            YooAssets.DestroyPackage(_packageName);
            YooAssets.Destroy();
        }

        public IEnumerator Start() {
            // 初始化
            YooAssets.Initialize();
            
            var packageName = _packageName;
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null) {
                package = YooAssets.CreatePackage(packageName);
            }
            
            InitializationOperation initializationOperation = null;
            if (_playMode == EPlayMode.OfflinePlayMode) {
                var initParameters = new OfflinePlayModeParameters();
                initializationOperation = package.InitializeAsync(initParameters);
                yield return initializationOperation;
            }
            
            if (_playMode == EPlayMode.EditorSimulateMode) {
                var initParameters = new EditorSimulateModeParameters();
                string buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline.ToString();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
                initializationOperation = package.InitializeAsync(initParameters);
                yield return initializationOperation;
            }
            
                    
            if (initializationOperation.Status != EOperationStatus.Succeed) {
                Debug.LogWarning($"{initializationOperation.Error}");
                yield break;
            }
            
            var version = initializationOperation.PackageVersion;
            Debug.Log($"Init resource package version : {version}");
            
            var defaultPackage = YooAssets.GetPackage(packageName);
        
            var operation = defaultPackage.UpdatePackageVersionAsync();
            yield return operation;

            if (operation.Status != EOperationStatus.Succeed) {
                Debug.LogWarning($"{operation.Error}");
            }
        
            Debug.Log($"update resource package version : {operation.PackageVersion}");
        
            bool savePackageVersion = true;
            var operation2 = defaultPackage.UpdatePackageManifestAsync(version, savePackageVersion);
            yield return operation2;

            if (operation2.Status != EOperationStatus.Succeed) {
                Debug.LogWarning($"{operation.Error}");
                yield break;
            }

            int downloadingMaxNum = 10;
            int failedTryAgain = 0;
            var downloader = defaultPackage.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (downloader.TotalDownloadCount == 0) {
                Debug.Log("No downloading resources are available");
            }
            else {
                Debug.Log("Downloading resources are available 单机不应该发生");
                yield break;
            }
        
            YooAssets.SetDefaultPackage(defaultPackage);
        
            Debug.Log("sample loaded");
        }
        public AssetHandle LoadAssetAsync(string assetName) {
            return YooAssets.LoadAssetAsync<GameObject>(assetName);
        }
        
        public SceneHandle LoadSceneAsync(string sceneName) {
            return YooAssets.LoadSceneAsync(sceneName);
        }
    }
}
