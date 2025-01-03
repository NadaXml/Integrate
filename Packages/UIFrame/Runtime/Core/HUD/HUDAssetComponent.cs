using UnityEngine;
using YooAsset;
namespace Core.HUD {
    public struct HUDAssetComponent<T> where T : MonoBehaviour {
        public AssetHandle AssetHandle;
        public GameObject InstantiatedGameObject;
        public T Binder;
        public RectTransform RectTransform;
        public bool IsDestroyed;
    }
}
