using System.Reflection;
using UnityEngine;
namespace Core.HUD {
    public class HUDBase<T> : IHUD<T> where T : MonoBehaviour {

        public struct HUDCreateParam {
            public HUDComponent Component;
        }
        
        public HUDComponent Component
        {
            get;
            set;
        }
        public HUDAssetComponent<T> Asset
        {
            get;
            set;
        }
        public HUDRandomComponent Random
        {
            get;
            set;
        }

        public HUDBase(HUDCreateParam param) {
            Component = param.Component;
        }
        
        public void Awake() {
        }
        public void Release() {
            Asset.AssetHandle?.Release();
            Asset = new HUDAssetComponent<T>() {
                IsDestroyed = true
            };

            if (Asset.InstantiatedGameObject != null) {
                if (this is HUDBase<HUDBinderCanvas>) {
                    HUDBinderCanvas hud = Asset.Binder as HUDBinderCanvas;
                    hud.DetachFromImageCanvas();
                    hud.DetachFromTextCanvas();
                }
                // 可以支持pool
                GameObject.Destroy(Asset.InstantiatedGameObject);
            }
        }
    }
}
