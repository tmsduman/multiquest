using Infrastructure.AssetBundleManager.Config;

namespace Infrastructure.AssetBundleManager.MonoBehaviours
{
    public class AssetBundleManagerSingleton : UnityEngine.MonoBehaviour
    {
        public static AssetBundleManager AssetBundleManager;

        public AssetBundleManagerConfig Config;

		private static bool initialized;

        private void Awake()
        {
			if (initialized)
				return;
			initialized = true;

            AssetBundleManager = new AssetBundleManager(this.Config, this);
        }
    }
}
