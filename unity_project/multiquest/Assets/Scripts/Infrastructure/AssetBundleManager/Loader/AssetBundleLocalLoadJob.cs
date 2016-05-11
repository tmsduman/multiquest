namespace Infrastructure.AssetBundleManager.Loader
{
    public class AssetBundleLocalLoadJob : AssetBundleLoadJob
    {
        public AssetBundleLocalLoadJob(
            UnityEngine.MonoBehaviour monoBehaviour, 
            string rootFolderName, 
            string assetName, 
            System.Action<AssetBundleLoadJob> callback,
            int version = -1)
            : base (
                monoBehaviour,
                () => 
                {
                    string basePath;
                    switch (UnityEngine.Application.platform)
                    {
                        case UnityEngine.RuntimePlatform.Android:
                            basePath = "jar:file://" + UnityEngine.Application.dataPath + "!/assets/";
                            break;

                        case UnityEngine.RuntimePlatform.IPhonePlayer:
                            basePath = "file://" + UnityEngine.Application.dataPath + "/Raw/";
                            break;

                        case UnityEngine.RuntimePlatform.WindowsWebPlayer:
                        case UnityEngine.RuntimePlatform.OSXWebPlayer:
                            basePath = UnityEngine.Application.dataPath + "/StreamingAssets/";
                            break;

                        default:
                            basePath = "file://" + UnityEngine.Application.dataPath + "/StreamingAssets/";
                            break;
                    }

                    return basePath + rootFolderName + "/" + assetName;
                },
                assetName,
                callback,
                version)
        {
        }
    }
}
