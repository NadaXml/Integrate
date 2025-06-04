namespace adt {
    public enum GameProcedure {
        None = 0,
        // asset service
        OfflinePlayMode, 
        EditorPlayMode,
        UpdatePackageVersion,
        UpdatePackageManifest,
        
        AssetLoad,
        
        Cancel,
        Success,
    }
}
