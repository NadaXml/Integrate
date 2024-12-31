using UnityEngine;
namespace Core.HUD {
    public interface IHUD<T> where T : MonoBehaviour {
        HUDComponent Component { get; set; }
        
        HUDAssetComponent<T> Asset { get; set; }
        
        HUDRandomComponent Random { get; set; }

        void Awake();
        
        void Release();
    }
}
