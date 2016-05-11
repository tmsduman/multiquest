using Infrastructure.AssetBundleManager.Config;
using Infrastructure.AssetBundleManager.Loader;

namespace Infrastructure.AssetBundleManager
{
    /// <summary>
    /// Tool to download asset bundles from remote or local path
    /// </summary>
    public class AssetBundleManager
    {
        /// <summary>
        /// Fired if an loading error occurs. First parameter is the asset name and second is the error message
        /// </summary>
        public event System.Action<string, string> LoadingErrorOccured;

        /// <summary>
        /// Fired if loading root folder manifest is finished
        /// </summary>
        public event System.Action RootFolderManifestLoaded;

        public delegate void LoadFinishedCallback<T>(System.Collections.Generic.Dictionary<string, T[]> loadedAssets, object[] arguments);

        private readonly AssetBundleLoader loader;

        /// <summary>
        /// Creates a new instance of the asset bundle manager
        /// </summary>
        /// <param name="config">Configuration parameters</param>
        /// <param name="monoBehaviour">Needed for the coroutines for the www download</param>
        public AssetBundleManager(AssetBundleManagerConfig config, UnityEngine.MonoBehaviour monoBehaviour)
        {
            // Create new asset bundle loader
            this.loader = new AssetBundleLoader(
                config,
                monoBehaviour);

            // Register for loading error event
            this.loader.LoadingErrorOccured += (assetName, errorMessage) =>
            {
                // Notify listener
                var tempEvent = this.LoadingErrorOccured;
                if (tempEvent != null)
                    tempEvent(assetName, errorMessage);
            };

            // Register for loading root folder manifest finished
            this.loader.RootFolderManifestLoaded += () =>
            {
                // Notify listener
                var tempEvent = this.RootFolderManifestLoaded;
                if (tempEvent != null)
                    tempEvent();
            };
        }

        /// <summary>
        /// Loads an asset from server or local path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">name of the asset</param>
        /// <param name="callback">function which is called when assets are downloaded</param>
        /// <param name="arguments">addtionial arguments, not used for loading</param>
        public void Load<T>(string assetName, AssetBundleLoader.LoadFinishedCallback<T> callback, object[] arguments = null, bool ignoreCaching = false)
        {
            this.loader.Load(assetName, callback, arguments, ignoreCaching);
        }

        /// <summary>
        /// Loads an array of assets from server or local path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetNames">names of the asset</param>
        /// <param name="callback">function which is called when assets are downloaded</param>
        /// <param name="arguments">addtionial arguments, not used for loading</param>
        public void Load<T>(string[] assetNames, LoadFinishedCallback<T> callback, object[] arguments = null, bool ignoreCaching = false)
        {
            int loadedAssetCount = 0;
            System.Collections.Generic.Dictionary<string, T[]> loadedAssets = 
                new System.Collections.Generic.Dictionary<string, T[]>();

            // Remove duplicated entries
            System.Collections.Generic.List<string> distinctList = new System.Collections.Generic.List<string>();
            foreach (var assetName in assetNames)
            {
                if (!distinctList.Contains(assetName))
                    distinctList.Add(assetName);
            }

            // Load every asset on his own
            for (int i = 0; i < distinctList.Count; i++)
            {
                this.loader.Load(
                    distinctList[i],
                    (T[] loadJobAssets, object[] loadArguments) =>
                    {
                        // Increment loaded asset count
                        loadedAssetCount++;

                        // Add load job assets to loaded assets
                        if (!loadedAssets.ContainsKey(loadArguments[0] as string))
                            loadedAssets.Add(loadArguments[0] as string, loadJobAssets);
                        
                        // If all assets are loaded call callback
                        if (loadedAssetCount >= distinctList.Count)
                        {
                            var tempEvent = callback;
                            if (tempEvent != null)
                                tempEvent(loadedAssets, arguments);
                        }
                    },
                    new object[] { distinctList[i] }, 
                    ignoreCaching);
            }
        }

        /// <summary>
        /// Loads a scene from server or local path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">name of the asset</param>
        /// <param name="callback">function which is called when scene are downloaded</param>
        /// <param name="arguments">addtionial arguments, not used for loading</param>
        public void LoadScene(string assetName, AssetBundleLoader.LoadFinishedCallback<string> callback, object[] arguments = null, bool ignoreCaching = false)
        {
            this.loader.Load(assetName, callback, arguments, ignoreCaching);
        }

        /// <summary>
        /// Loads an array of scenes from server or local path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetNames">names of the asset</param>
        /// <param name="callback">function which is called when scenes are downloaded</param>
        /// <param name="arguments">addtionial arguments, not used for loading</param>
        public void LoadScene(string[] assetNames, LoadFinishedCallback<string> callback, object[] arguments = null, bool ignoreCaching = false)
        {
            int loadedAssetCount = 0;
            System.Collections.Generic.Dictionary<string, string[]> loadedAssets =
                new System.Collections.Generic.Dictionary<string, string[]>();

            // Load every asset on his own
            foreach (var assetName in assetNames)
            {
                this.loader.Load(
                    assetName,
                    (string[] loadJobAssets, object[] loadArguments) =>
                    {
                        // Increment loaded asset count
                        loadedAssetCount++;

                        // Add load job assets to loaded assets
                        loadedAssets.Add(loadArguments[0] as string, loadJobAssets);

                        // If all assets are loaded call callback
                        if (loadedAssetCount >= assetNames.Length)
                        {
                            var tempEvent = callback;
                            if (tempEvent != null)
                                tempEvent(loadedAssets, arguments);
                        }
                    },
                    new object[] { assetName },
                    ignoreCaching);
            }
        }

        #region versioning

        /// <summary>
        /// Sets the versions of assets
        /// </summary>
        public void SetAssetVersions(System.Collections.Generic.Dictionary<string, int> assetVersions)
        {
            this.loader.SetAssetVersions(assetVersions);
        }

        /// <summary>
        /// Set the version of the asset
        /// </summary>
        public void SetAssetVersion(string assetName, int version)
        {
            this.loader.SetAssetVersion(assetName, version);
        }

        #endregion
    }
}
