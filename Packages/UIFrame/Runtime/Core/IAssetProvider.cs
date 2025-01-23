using YooAsset;
namespace UIFrame.Core {
    public interface IAssetProvider {
        AssetHandle LoadAssetAsync<TObject>(string assetName) where TObject : UnityEngine.Object;
    }
}
