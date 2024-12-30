namespace Core.HUD {
    public interface IHUD {
        HUDComponent Component { get; set; }
        
        HUDAssetComponent Asset { get; set; }
        
        HUDRandomComponent Random { get; set; }

        void Awake();
        
        void Release();
    }
}
