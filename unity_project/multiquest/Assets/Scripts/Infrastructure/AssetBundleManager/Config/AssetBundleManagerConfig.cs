namespace Infrastructure.AssetBundleManager.Config
{
    [System.Serializable]
    public class AssetBundleManagerConfig
    {
        [UnityEngine.Tooltip("Name of the root folder of asset bundles.")]
        public string RootFolderName;

        [UnityEngine.Tooltip("If true asset bundle will be load from local path.")]
        public bool LoadLocalOnly;
        public bool LoadLocalFirst;

        [UnityEngine.Tooltip("URL from the server where the asset bundles are present (excluding the root folder and no slash at the end).")]
        public string RemoteServerUrl;

        [UnityEngine.Tooltip("If true all asset bundles will be loaded with cache, so versions have to be added to the manager.")]
        public bool EnableLoadCaching;
    }
}
