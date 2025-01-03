using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using YooAsset;

public class Launcher : MonoBehaviour {
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    public string PackageName = "DefaultPackage";
    
    IEnumerator Start() {
        
        DontDestroyOnLoad(gameObject);
        
        YooAssets.Initialize();

        var packageName = PackageName;
        var package = YooAssets.TryGetPackage(packageName);
        if (package == null) {
            package = YooAssets.CreatePackage(packageName);
        }

        InitializationOperation initializationOperation = null;
        if (PlayMode == EPlayMode.OfflinePlayMode) {
            var initParameters = new OfflinePlayModeParameters();
            initializationOperation = package.InitializeAsync(initParameters);
            yield return initializationOperation;
        }

        if (PlayMode == EPlayMode.EditorSimulateMode) {
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

        var assetHandle = YooAssets.LoadAssetAsync<GameObject>("Assets/UIDocument/Res/Misc/UIDocument.prefab");
        yield return assetHandle;

        var instantiateOperation = assetHandle.InstantiateAsync();
        yield return instantiateOperation;

        instantiateOperation.Result.transform.SetParent(GameObject.Find("Enter").transform);

        var sceneHandle = defaultPackage.LoadSceneAsync("Assets/UIDocument/Scene/HUD.unity");
        yield return sceneHandle;
        
        Debug.Log("sample loaded");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnDestroy() {
        // var package = YooAssets.GetPackage("DefaultPackage");
        // package.ClearAllCacheFilesAsync()
    }
}
