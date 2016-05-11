namespace Infrastructure.AssetBundleManager.Loader
{
    public class AssetBundleRemoteLoadJob : AssetBundleLoadJob
    {
        public AssetBundleRemoteLoadJob(
            UnityEngine.MonoBehaviour monoBehaviour, 
            string remoteServerUrl, 
            string rootFolderName, 
            string assetName,
            System.Action<AssetBundleLoadJob> callback,
            int version = -1)
            : base(
                monoBehaviour,
                () => 
                {
                    return remoteServerUrl + "/" + rootFolderName + "/" + assetName;
                },
                assetName, 
                callback,
                version)
        {
        }
    }
}
