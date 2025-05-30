using adt;
using YooAsset;
using Cysharp.Threading.Tasks;
using game_logic;
using System;
using System.Threading;
using Debug = UnityEngine.Debug;

namespace asset_service {
    /// <summary>
    /// 引用YooAsset包装的服务
    /// </summary>
    public class Asset : GameService {
        
        [Serializable]
        public struct AssetParam {
            public string packageName;
            public EPlayMode playMode;
        }
        
        AssetParam param;
        
        public async UniTask<GameProcedure> Init(AssetParam inParam, CancellationTokenSource cts) {
            param = inParam;

            GameProcedure ret = GameProcedure.None;
            
            YooAssets.Initialize();

            var package = YooAssets.TryGetPackage(param.packageName);
            if (package == null) {
                package = YooAssets.CreatePackage(param.packageName);
            }
            
            InitializationOperation initializationOperation = null;
            // 单机资源包模式
            if (param.playMode == EPlayMode.OfflinePlayMode) {
                ret = GameProcedure.OfflinePlayMode;
                var initParameters = new OfflinePlayModeParameters();
                initializationOperation = package.InitializeAsync(initParameters);
                await initializationOperation.ToUniTask();
            } 

            // Editor模式
            if (param.playMode == EPlayMode.EditorSimulateMode) {
                ret = GameProcedure.EditorPlayMode;
                var initParameters = new EditorSimulateModeParameters();
                string buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline.ToString();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipeline, param.packageName);
                initializationOperation = package.InitializeAsync(initParameters);
                await initializationOperation.ToUniTask();
            }
            
            // 联机运行模式，再等等
            // if (playMode == EPlayMode.HostPlayMode)
            // {
            //     string defaultHostServer = HostServerURL;
            //     string fallbackHostServer = FallbackHostServerURL;
            //     var createParameters = new HostPlayModeParameters();
            //     createParameters.CacheBootVerifyLevel = VerifyLevel;
            //     createParameters.DecryptionServices = new FileStreamDecryption();
            //     createParameters.BuildinQueryServices = new GameQueryServices();
            //     createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            //     initializationOperation = package.InitializeAsync(createParameters);
            // }

            if (initializationOperation.Status != EOperationStatus.Succeed) {
                Debug.LogError($"{initializationOperation.Error}");
                return ret;
            }

            if (cts.IsCancellationRequested) {
                return GameProcedure.Cancel;
            }

            ret = GameProcedure.UpdatePackageVersion;
            
            var version = initializationOperation.PackageVersion;
            Debug.Log($"Init resource package version : {version}");

            var defaultPackage = YooAssets.GetPackage(package.PackageName);
            var updatePackageOperation = defaultPackage.UpdatePackageVersionAsync();
            await updatePackageOperation.ToUniTask();

            if (updatePackageOperation.Status != EOperationStatus.Succeed) {
                Debug.LogWarning($"{updatePackageOperation.Error}");
                return ret;
            }
            
            if (cts.IsCancellationRequested) {
                return GameProcedure.Cancel;
            }
            
            Debug.Log($"update resource package version : {updatePackageOperation.PackageVersion}");

            ret = GameProcedure.UpdatePackageManifest;

            bool savePackageVersion = true;
            var updateManifestOperation = defaultPackage.UpdatePackageManifestAsync(version, savePackageVersion);
            await updateManifestOperation.ToUniTask();
            
            if (updateManifestOperation.Status != EOperationStatus.Succeed) {
                Debug.LogWarning($"{updateManifestOperation.Error}");
                return ret;
            }
            
            if (cts.IsCancellationRequested) {
                return GameProcedure.Cancel;
            }
            
            int downloadingMaxNum = 10;
            int failedTryAgain = 0;
            var downloader = defaultPackage.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            
            if (downloader.TotalDownloadCount == 0) {
                Debug.Log("No downloading resources are available");
            }
            
            YooAssets.SetDefaultPackage(defaultPackage);
            
            Debug.Log(" asset service ok");

            return GameProcedure.Success;
        }
        
        public ResHandle LoadAssetAsync<TObject>(string assetName) where TObject : UnityEngine.Object {
            var assetHandle = YooAssets.LoadAssetAsync<TObject>(assetName);
            return new ResHandle() {
                assetHandle = assetHandle
            };
        }

        public void UnloadAsset(string assetName) {
            var package = YooAssets.GetPackage(param.packageName);
            package.TryUnloadUnusedAsset(assetName);
        }
        
        // public ResHandle LoadSceneAsync(string sceneName) {
        //     var sceneHandle = YooAssets.LoadSceneAsync(sceneName);
        //     return new ResHandle() {
        //         sceneHandle = sceneHandle
        //     };
        // }
    }
}