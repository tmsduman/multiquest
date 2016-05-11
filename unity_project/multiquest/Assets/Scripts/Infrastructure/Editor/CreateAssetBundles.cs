using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    private const string ExportPath = "StreamingAssets/Export";

    [MenuItem ("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles ()
    {
        string path = Application.dataPath + "/" + ExportPath;
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}