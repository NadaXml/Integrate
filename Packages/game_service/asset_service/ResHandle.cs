using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using YooAsset;
namespace asset_service {
    public class ResHandle : IDisposable {
        public AssetHandle assetHandle;

        public UniTask ToUniTask() {
            return assetHandle.ToUniTask();
        }

        public T GetAsset<T>() where T : UnityEngine.Object {
            var asset = assetHandle.AssetObject as T;
            return asset;
        }
        public void Dispose() {
            assetHandle.Release();
        }

        public bool IsSucceed() {
            return assetHandle.Status == EOperationStatus.Succeed;
        }
    }
}
