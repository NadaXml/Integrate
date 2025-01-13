using YooAsset;
namespace UIFrame.Core {
    public interface IAssetProvider {
        AssetHandle LoadAssetAsync(string assetName);
    }
}
