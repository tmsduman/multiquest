namespace Infrastructure.AssetBundleManager.Loader
{
    public class AssetBundleLoadJob
    {
        public bool Loading { get; private set; }
        public int Version { get; private set; }
        public string AssetName { get; private set; }
        public UnityEngine.Object[] LoadedAssets { get; private set; }
        public string ErrorMessage { get; private set; }
        public UnityEngine.AssetBundle AssetBundle { get; private set; }

        public AssetBundleLoadJob(
            UnityEngine.MonoBehaviour monoBehaviour, 
            System.Func<string> getUrlFunction,
            string assetName, 
            System.Action<AssetBundleLoadJob> callback,
            int version = -1)
        {
            this.AssetName = assetName;
            this.Version = version;

            if (monoBehaviour != null)
            {
                this.Loading = true;
                if (version == -1)
                {
                    // No version set so load without cache
                    monoBehaviour.StartCoroutine(this.Load(getUrlFunction(), callback));
                }
                else
                {
                    // Version is set so load with cache
                    monoBehaviour.StartCoroutine(this.LoadCached(getUrlFunction(), version, callback));
                }
            }

        }

        /// <summary>
        /// Load from url without caching
        /// </summary>
        protected System.Collections.IEnumerator Load(string url, System.Action<AssetBundleLoadJob> callback)
        {
            //UnityEngine.Debug.Log(url);
            UnityEngine.WWW request = new UnityEngine.WWW(url);

            while (!request.isDone)
                yield return request;

            this.RequestFinished(request, callback);
        }

        /// <summary>
        /// Load from url with caching
        /// </summary>
        protected System.Collections.IEnumerator LoadCached(string url, int version, System.Action<AssetBundleLoadJob> callback)
        {
            UnityEngine.Debug.Log("load cached " + url + " version " + version);
            UnityEngine.WWW request = UnityEngine.WWW.LoadFromCacheOrDownload(url, version);
            
            while (!request.isDone)
                yield return request;

            this.RequestFinished(request, callback);
        }

        /// <summary>
        /// Request is finished so get asset bundles
        /// </summary>
        private void RequestFinished(UnityEngine.WWW request, System.Action<AssetBundleLoadJob> callback)
        {
            this.Loading = false;

            if (!string.IsNullOrEmpty(request.error))
            {
                // An error occured so save the error
                this.ErrorMessage = "Asset bundle load failed (URL: " + request.url + "): " + request.error;
                this.LoadedAssets = new UnityEngine.Object[0];
            }

            if (request.assetBundle != null)
            {
                this.AssetBundle = request.assetBundle;

                // Response have an asset bundle so load all of them
                this.LoadedAssets = this.AssetBundle.LoadAllAssets();

                // hacky fix to show precompiled shader from asset bundle in editor
#if UNITY_EDITOR

                foreach (var asset in this.LoadedAssets)
                {
                    if (asset is UnityEngine.Material)
                    {
                        var mat = asset as UnityEngine.Material;
                        mat.shader = UnityEngine.Shader.Find(mat.shader.name);
                    }

                    if (asset is UnityEngine.GameObject)
                    {
                        var go = asset as UnityEngine.GameObject;

                        // Fix renderers.
                        var rendererArray = go.GetComponentsInChildren<UnityEngine.Renderer>(true);
                        foreach (var renderer in rendererArray)
                        {
                            if (renderer.sharedMaterial != null &&
                                renderer.sharedMaterial.shader != null)
                            {
                                renderer.sharedMaterial.shader =
                                    UnityEngine.Shader.Find(renderer.sharedMaterial.shader.name);
                            }
                        }

                        // Fix UI graphics.
                        var graphics = go.GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
                        foreach (var graphic in graphics)
                        {
                            if (graphic.material != null && graphic.material.shader != null)
                            {
                                graphic.material.shader =
                                    UnityEngine.Shader.Find(graphic.material.shader.name);
                            }
                        }
                    }
                }
#endif

            }
            else
            {
                // Response have no asset bundle so save an error
                this.ErrorMessage = "No Asset bundle can be load from URL: " + request.url;
                this.LoadedAssets = new UnityEngine.Object[0];
            }

            // Call callback
            var tempEvent = callback;
            if (tempEvent != null)
                tempEvent(this);
        }
    }
}
