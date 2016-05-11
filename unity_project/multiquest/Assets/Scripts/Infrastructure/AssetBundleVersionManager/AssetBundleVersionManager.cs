using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Infrastructure.AssetBundleManager.MonoBehaviours;
using UnityEngine;

namespace Infrastructure.AssetBundleVersionManager
{
    public class AssetBundleVersionManager : MonoBehaviour
    {
        public static event System.Action VersionFileLoaded;

        [SerializeField] private string versioningURL;

        private void Start()
        {
            if (!string.IsNullOrEmpty(this.versioningURL))
                this.StartCoroutine(this.LoadVersioningFile(this.versioningURL));
        }

        private IEnumerator LoadVersioningFile(string url)
        {
            WWW request = new WWW(url);

            while (!request.isDone)
                yield return request;

            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError("Downloading version list failed (URL: " + url + "): " + request.error);
                yield break;
            }

            if (string.IsNullOrEmpty(request.text))
            {
                Debug.LogError("Version list text is empty (URL: " + url + ").");
                yield break;
            }

            this.ParseVersionFile(request.text);
        }

        private void ParseVersionFile(string text)
        {
            Dictionary<string, int> assetVersions = new Dictionary<string, int>();

            string assetName;
            int assetVersion;

            XmlReader reader = XmlReader.Create(new StringReader(text));
            while (reader.ReadToFollowing("asset"))
            {
                reader.ReadToFollowing("name");
                assetName = reader.ReadElementContentAsString();
                reader.ReadToFollowing("version");
                assetVersion = reader.ReadElementContentAsInt();

                assetVersions.Add(assetName, assetVersion);
            }

            AssetBundleManagerSingleton.AssetBundleManager.SetAssetVersions(assetVersions);

            var tempEvent = VersionFileLoaded;
            if (tempEvent != null)
                tempEvent();
        }
    }
}
