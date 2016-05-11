using System;
using System.Collections.Generic;
using Infrastructure.AssetBundleManager.Config;
using UnityEngine;

namespace Infrastructure.AssetBundleManager.Loader
{
    public class AssetBundleLoader
    {
        /// <summary>
        /// Fired if an loading error occurs. First parameter is the asset name and second is the error message
        /// </summary>
        public event Action<string, string> LoadingErrorOccured;

        /// <summary>
        /// Fired if loading root folder manifest is finished
        /// </summary>
        public event Action RootFolderManifestLoaded;

        public delegate void LoadFinishedCallback<T>(T[] loadedAssets, object[] arguments);

        // Stores all dependencies of an asset
        private class AssetDependencies
        {
            public string AssetName;
            public List<string> Dependencies;
            public bool IgnoreCaching;
        }

        // Stores all data regarding to a loading finished callback
        private struct CallbackData
        {
            public Delegate Delegate;
            public Type Type;
            public object[] Arguments;
        }

        private readonly AssetBundleManagerConfig config;
        private readonly MonoBehaviour monoBehaviour;

        // All already loaded assets are cached for faster access
        private Dictionary<AssetBundleLoadJob, List<CallbackData>> cachedLoadJobs =
            new Dictionary<AssetBundleLoadJob, List<CallbackData>>();

        // Versions of assets used when cached downloads are enabled through config
        private Dictionary<string, int> assetVersions = new Dictionary<string,int>();

        // Root folder manifest to detect asset dependencies
        private AssetBundleManifest rootFolderManifest;

        /// <summary>
        /// Creates a new asset bundle loader and loads the root folder manifest.
        /// </summary>
        /// <param name="config">Configuration parameters</param>
        /// <param name="monoBehaviour">Needed for the coroutines for the www download</param>
        public AssetBundleLoader(
            AssetBundleManagerConfig config,
            UnityEngine.MonoBehaviour monoBehaviour)
        {
            this.config = config;
            this.monoBehaviour = monoBehaviour;
            
            // load root folder manifest
            this.Load<AssetBundleManifest>(
                this.config.RootFolderName,
                (loadedAssets, arguments) => 
                {
                    if (loadedAssets.Length > 0)
                    {
                        this.rootFolderManifest = loadedAssets[0];
                        
                        // Notify listener
                        var tempEvent = this.RootFolderManifestLoaded;
                        if (tempEvent != null)
                            tempEvent();
                    }
                    else
                        Debug.LogError("No asset bundle manifest found for root folder " + this.config.RootFolderName);
                },
                null,
                true,
                true);
        }

        #region load asset

        /// <summary>
        /// Loads an asset from server or local path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">name of the asset</param>
        /// <param name="callback">function which is called when assets are downloaded</param>
        /// <param name="arguments">addtionial arguments, not used for loading</param>
        public void Load<T>(string assetName, LoadFinishedCallback<T> callback, object[] arguments, bool ignoreCaching = false, bool ignoreToLower = false)
        {
            // Asset bundles all have names with lower letters
            if (!ignoreToLower)
                assetName = assetName.ToLower();

            if (!ignoreCaching)
            {
                // Check if asset is already loaded
                foreach (var loadJob in this.cachedLoadJobs)
                {
                    if (loadJob.Key.AssetName.Equals(assetName))
                    {
                        // If still loading add callback
                        if (loadJob.Key.Loading)
                        {
                            foreach (var callbackData in loadJob.Value)
                            {
                                // If callback already added skip it
                                if (callbackData.Delegate.Equals(callback))
                                {
                                    return;
                                }
                            }

                            // Add new load finished callback
                            loadJob.Value.Add(new CallbackData()
                            {
                                Delegate = callback,
                                Type = typeof(T),
                                Arguments = arguments
                            });
                        }
                        else
                        {
                            // Asset is loaded so execute callback
                            this.ExecuteCallback(callback, typeof(T), arguments, loadJob.Key);
                        }
                        return;
                    }
                }
            }

            // Check if any dependecies has to loaded first
            if (!this.LoadDependencies<T>(assetName, callback, arguments, ignoreCaching))
            {
                // No dependencies there or all already loaded so loading can start
                this.cachedLoadJobs.Add(
                    this.CreateLoadJob(assetName), 
                    new List<CallbackData>(){ new CallbackData()
                    { 
                        Delegate = callback, 
                        Type = typeof(T),
                        Arguments = arguments
                    }});
            }
        }

        #region create load job

        /// <summary>
        /// Creates a local or remote load job for the asset name depending on the initial given config
        /// </summary>
        private AssetBundleLoadJob CreateLoadJob(string assetName, bool ignoreCaching = false)
        {
            if (this.config.LoadLocalOnly)
            {
                // Load assets from local path
                return this.CreateLocalLoadJob(
                    assetName,
                    this.config.EnableLoadCaching && !ignoreCaching ? this.GetAssetVersion(assetName) : -1);
            }
            else
            {
                // Load assets from remote
                return this.CreateRemoteLoadJob(
                    assetName,
                    this.config.EnableLoadCaching && !ignoreCaching ? this.GetAssetVersion(assetName) : -1);
            }
        }

        /// <summary>
        /// Creates a local load job
        /// </summary>
        private AssetBundleLocalLoadJob CreateLocalLoadJob(string assetName, int version = -1)
        {
            return new AssetBundleLocalLoadJob(
                this.monoBehaviour,
                this.config.RootFolderName,
                assetName,
                this.LoadingFinished,
                version);
        }

        /// <summary>
        /// Creates a remote load job
        /// </summary>
        private AssetBundleRemoteLoadJob CreateRemoteLoadJob(string assetName, int version = -1)
        {
            return new AssetBundleRemoteLoadJob(
                this.monoBehaviour,
                this.config.RemoteServerUrl,
                this.config.RootFolderName,
                assetName,
                this.LoadingFinished,
                version);
        }

        #endregion

        /// <summary>
        /// Executes all related callbacks of a finished load job
        /// </summary>
        /// <param name="loadJob">Finished load job</param>
        private void LoadingFinished(AssetBundleLoadJob loadJob)
        {
            if (this.cachedLoadJobs.ContainsKey(loadJob))
            {
                // Check if an error has occured
                if (!string.IsNullOrEmpty(loadJob.ErrorMessage))
                {
                    // An error occured so notify listener
                    Debug.LogError(loadJob.ErrorMessage);
                    var tempEvent = this.LoadingErrorOccured;
                    if (tempEvent != null)
                        tempEvent(loadJob.AssetName, loadJob.ErrorMessage);
                } 

                // Execute all stored callbacks
                foreach (var callbackData in this.cachedLoadJobs[loadJob])
                {
                    this.ExecuteCallback(callbackData.Delegate, callbackData.Type, callbackData.Arguments, loadJob);
                }

                // Clear stored callbacks to save memory
                this.cachedLoadJobs[loadJob].Clear();

                // Check if an error has occured
                if (!string.IsNullOrEmpty(loadJob.ErrorMessage))
                {
                    // An error occured so delete load job from cache
                    this.cachedLoadJobs.Remove(loadJob);
                }
            }
        }

        /// <summary>
        /// Executes a load job callback and filters and casts the loaded assets by the callback type
        /// </summary>
        private void ExecuteCallback(Delegate callback, Type callbackGenericType, object[] arguments, AssetBundleLoadJob loadJob)
        {
            // if type is string scene were loaded so return scene pathes
            if (callbackGenericType == typeof(string))
            {
                // Execute callback
                var tempEvent = callback;
                if (tempEvent != null)
                    tempEvent.DynamicInvoke(loadJob.AssetBundle.GetAllScenePaths(), arguments);
                return;
            }

            // Detect which loaded assets are of the given callback type
            List<UnityEngine.Object> castableAssets = new List<UnityEngine.Object>();
            foreach (var asset in loadJob.LoadedAssets)
            {
                if (asset.GetType().Equals(callbackGenericType))
                    castableAssets.Add(asset);
            }

            // Create an array of all castable assets
            Array array = Array.CreateInstance(callbackGenericType, castableAssets.Count);
            for (int i = 0; i < castableAssets.Count; i++)
            {
                array.SetValue(castableAssets[i], i);
            }

            // Execute callback
            var tempCallback = callback;
            if (tempCallback != null)
                tempCallback.DynamicInvoke(array, arguments);
        }

        #region dependencies

        /// <summary>
        /// Detects if the asset has dependencies and load them first if they are not already loaded
        /// </summary>
        private bool LoadDependencies<T>(
            string assetName, 
            LoadFinishedCallback<T> callback,
            object[] arguments, 
            bool ignoreCaching = false)
        {
            // Root folder manifest must be present to detect dependencies
            if (this.rootFolderManifest != null)
            {
                AssetDependencies assetDependencies = null;
                
                // Get all dependencies of the asset
                string[] dependencies = this.rootFolderManifest.GetAllDependencies(assetName);
                if (dependencies.Length > 0)
                {
                    // Check for every dependency if already loaded
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        bool alreadyLoaded = false;
                        foreach (var cachedLoadJob in this.cachedLoadJobs)
                        {
                            // Dependecy is already loaded
                            if (cachedLoadJob.Key.AssetName.Equals(dependencies[i]))
                            {
                                alreadyLoaded = true;
                                break;
                            }
                        }

                        if (!alreadyLoaded || ignoreCaching)
                        {
                            // Dependency had to be loaded

                            // Create asset dependencies class if not already done
                            if (assetDependencies == null)
                            {
                                assetDependencies = new AssetDependencies()
                                {
                                    AssetName = assetName,
                                    Dependencies = new List<string>(),
                                    IgnoreCaching = ignoreCaching
                                };
                            }
                            
                            // Store asset dependecy
                            assetDependencies.Dependencies.Add(dependencies[i]);

                            // Load dependency first
                            this.Load<UnityEngine.Object>(
                                dependencies[i], 
                                this.DependenciesLoaded<T>, 
                                new object[]{ 
                                    assetDependencies,
                                    callback,
                                    arguments});
                            
                            Debug.Log("load dependency for " + assetName + ": " + dependencies[i]);
                        }
                    }
                }

                return assetDependencies != null;
            }

            return false;
        }

        /// <summary>
        /// Checks if all dependcies are loaded and if yes starts asset loading
        /// </summary>
        private void DependenciesLoaded<T>(UnityEngine.Object[] loadedAssets, object[] arguments)
        {
            AssetDependencies assetDependencies = arguments[0] as AssetDependencies;

            // Check all dependencies of the asset
            foreach (var dependency in assetDependencies.Dependencies)
            {
                foreach (var cachedLoadJob in this.cachedLoadJobs)
                {
                    if (cachedLoadJob.Key.AssetName.Equals(dependency)) 
                    {
                        // A dependency still loads so we have to step out
                        if (cachedLoadJob.Key.Loading)
                            return;
                        break;
                    }
                }
            }

            // All dependencies loaded so load delayed asset
            this.Load<T>(
                assetDependencies.AssetName,
                (LoadFinishedCallback<T>)arguments[1],
                (object[])arguments[2],
                assetDependencies.IgnoreCaching);
        }

        #endregion

        #endregion

        #region versioning

        /// <summary>
        /// Returns the version of an asset if it is set otherwise it returns 0
        /// </summary>
        private int GetAssetVersion(string assetName)
        {
            if (this.assetVersions.ContainsKey(assetName))
                return this.assetVersions[assetName];
            return 0;
        }

        /// <summary>
        /// Sets the versions of assets
        /// </summary>
        public void SetAssetVersions(Dictionary<string, int> assetVersions)
        {
            foreach (var assetVersion in assetVersions)
            {
                this.SetAssetVersion(assetVersion.Key, assetVersion.Value);
            }
        }

        /// <summary>
        /// Sets the version of an asset
        /// </summary>
        public void SetAssetVersion(string assetName, int version)
        {
            if (this.assetVersions.ContainsKey(assetName))
                this.assetVersions[assetName] = version;
            else
                this.assetVersions.Add(assetName, version);
        }

        #endregion
    }
}
