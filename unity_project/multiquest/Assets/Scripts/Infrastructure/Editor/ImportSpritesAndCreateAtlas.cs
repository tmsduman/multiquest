using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

namespace UI.Editor
{
    public class ImportSpritesAndCreateAtlas
    {
        public static int DEFAULT_PADDING = 2;
        public static string DEFAULT_ATLAS_FILE_EXTENSION = ".png";

        private struct ProceedAtlasData
        {
            public string SavePath;
            public Texture2D Texture;
            public List<string> ImportedSpritePaths;
        }

        private static List<ProceedAtlasData> proceedAtlases;

        #region create atlas from packaging tag

        [MenuItem("Tools/Create Atlas From Packing Tag")]
        public static void OpenPackingTagInsertWindow()
        {
            PackingTagInsertWindow window =
                (PackingTagInsertWindow)EditorWindow.GetWindow(
                    typeof(PackingTagInsertWindow),
                    false,
                    "Create atlas from packing tag",
                    true);
            window.Show();
        }

        public static void CreateAtlasFromPackingTag(string packingTag, string savePath)
        {
            if(!savePath.EndsWith(DEFAULT_ATLAS_FILE_EXTENSION))
            {
                int lastDotIndex = savePath.LastIndexOf('.');
                if (lastDotIndex > 0)
                    savePath = savePath.Substring(0, lastDotIndex);
                savePath += DEFAULT_ATLAS_FILE_EXTENSION;
            }
            Debug.Log(savePath + " <- savePath");
            List<string> importedSpritePaths = new List<string>();

            string[] assetPaths = AssetDatabase.FindAssets("t:Sprite");
            foreach (var item in assetPaths)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(item);
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (textureImporter != null && textureImporter.spritePackingTag.Equals(packingTag))
                {
                    //textureImporter.spritePixelsPerUnit = 100;
                    //textureImporter.textureType = TextureImporterType.Sprite;
                    ////textureImporter.spritePivot = Vector2.zero;
                    //textureImporter.mipmapEnabled = false;
                    //textureImporter.filterMode = FilterMode.Bilinear;
                    //textureImporter.compressionQuality = 100;
                    //textureImporter.maxTextureSize = 2048;
                    textureImporter.anisoLevel = 0;
                    textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
					textureImporter.alphaIsTransparency = true;
					textureImporter.SaveAndReimport();
                    importedSpritePaths.Add(assetPath);
                }   
            }

            Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.ForceRegroup);

            proceedAtlases = new List<ProceedAtlasData>();

            int atlasCount = 0;
            foreach (var atlas in Packer.atlasNames)
            {
                if (atlas.Contains(packingTag))
                {
                    foreach (Texture2D atlasTexture in Packer.GetTexturesForAtlas(atlas))
                    {
                        if (atlasTexture != null)
                        {
                            ProceedAtlasData atlasData = new ProceedAtlasData();
                            atlasData.ImportedSpritePaths = importedSpritePaths;
                            if (atlasCount == 0)
                                atlasData.SavePath = savePath;
                            else
                                atlasData.SavePath = savePath.Substring(0, savePath.LastIndexOf('.')) + atlasCount + DEFAULT_ATLAS_FILE_EXTENSION;
                            atlasData.Texture = atlasTexture;
                            proceedAtlases.Add(atlasData);
                            atlasCount++;
                        }
                    }
                }
            }

            if (proceedAtlases.Count > 0 && importedSpritePaths.Count > 0)
                CheckProccedTextures();
            else
                Debug.LogError("No sprites with packing tag " + packingTag + " found. Creation aborted!");
        }

        #endregion

        #region proceed textures

        private static void CheckProccedTextures()
        {
            if (proceedAtlases.Count > 0)
            {
                ProceedAtlasData atlasData = proceedAtlases[0];
                proceedAtlases.RemoveAt(0);
                ProceedAtlasTexture(atlasData);
            }
            else
                Debug.Log("Atlases generated"); 
        }

        private static void ProceedAtlasTexture(ProceedAtlasData atlasData)
        {
            try
            {
                var spriteRects = new List<Rect>();
                var spritePixels = new List<Color[]>();

                List<SpriteMetaData> spriteMetaDatas = new List<SpriteMetaData>();
                for (int i = 0; i < atlasData.ImportedSpritePaths.Count; i++)
                {
                    SpriteMetaData metaData = new SpriteMetaData();
                    metaData.name = Path.GetFileNameWithoutExtension(atlasData.ImportedSpritePaths[i]);

                    EditorUtility.DisplayProgressBar(
                        "Updating Atlas",
                        metaData.name,
                        (float)i / atlasData.ImportedSpritePaths.Count);

                    Sprite importedSprite = AssetDatabase.LoadAssetAtPath(atlasData.ImportedSpritePaths[i], typeof(Sprite)) as Sprite;

                    if (importedSprite != null && importedSprite.packed)
                    {
                        if (atlasData.Texture != SpriteUtility.GetSpriteTexture(importedSprite, true))
                            continue;

                        var importedSpriteTI = AssetImporter.GetAtPath(atlasData.ImportedSpritePaths[i]) as TextureImporter;
                        if(importedSpriteTI.spriteImportMode == SpriteImportMode.Multiple)
                        {
                            metaData.border = importedSpriteTI.spritesheet[0].border;
                            metaData.alignment = importedSpriteTI.spritesheet[0].alignment;
                            metaData.pivot = importedSpriteTI.spritesheet[0].pivot;
                        }
                        else
                        {
                            var textureImporterSettings = new TextureImporterSettings();
                            importedSpriteTI.ReadTextureSettings(textureImporterSettings);

                            metaData.border = textureImporterSettings.spriteBorder;
                            metaData.alignment = textureImporterSettings.spriteAlignment;
                            metaData.pivot = textureImporterSettings.spritePivot;
                        }
                            
                        Rect spriteRect = new Rect(1, 1, 0, 0);
                        Vector2[] uvs = SpriteUtility.GetSpriteUVs(importedSprite, true);
                    
                        foreach (var item in uvs)
                        {
                            if (item.x < spriteRect.x)
                                spriteRect.x = item.x;
                            if (item.y < spriteRect.y)
                                spriteRect.y = item.y;
                            if (item.x > spriteRect.width)
                                spriteRect.width = item.x;
                            if (item.y > spriteRect.height)
                                spriteRect.height = item.y;
                        }

                        //Debug.Log("sprite rect " + spriteRect + " " + atlasSprite.texture.width + " " + atlasSprite.texture.height);

                        spriteRect.width = Mathf.RoundToInt((spriteRect.width - spriteRect.x) * atlasData.Texture.width);
                        spriteRect.height = Mathf.RoundToInt((spriteRect.height - spriteRect.y) * atlasData.Texture.height);
                        spriteRect.x *= Mathf.RoundToInt(atlasData.Texture.width);
                        spriteRect.y *= Mathf.RoundToInt(atlasData.Texture.height);
                        //Debug.Log("sprite rect " + spriteRect);

                        spriteRect.x = Mathf.Max(spriteRect.x, 0);
                        spriteRect.y = Mathf.Max(spriteRect.y, 0);

                        metaData.rect = spriteRect;

                        spriteMetaDatas.Add(metaData);

                        TextureImporter spriteImporter = TextureImporter.GetAtPath(atlasData.ImportedSpritePaths[i]) as TextureImporter;
                        TextureImporterFormat currentFormat = spriteImporter.textureFormat;
                        spriteImporter.textureFormat = TextureImporterFormat.RGBA32;
                        spriteImporter.isReadable = true;
                        AssetDatabase.ImportAsset(atlasData.ImportedSpritePaths[i]);

                        if (importedSprite.texture.width > spriteRect.width || importedSprite.texture.height > spriteRect.height)
                        {
                            //Debug.Log("sprite " + atlasData.ImportedSpritePaths[i] + " has to be trimmed");
                            Vector2 minValues = Vector2.zero;
                            for (int j = 0; j < importedSprite.texture.width; j++)
                            {
                                for (int k = 0; k < importedSprite.texture.height; k++)
                                {
                                    if (importedSprite.texture.GetPixel(j, k).a != 0)
                                    {
                                        minValues.x = Mathf.Max(j - DEFAULT_PADDING, 0);
                                        j = importedSprite.texture.width;
                                        break;
                                    }
                                }
                            }

                            for (int j = 0; j < importedSprite.texture.height; j++)
                            {
                                for (int k = 0; k < importedSprite.texture.width; k++)
                                {
                                    if (importedSprite.texture.GetPixel(k, j).a != 0)
                                    {
                                        minValues.y = Mathf.Max(j - DEFAULT_PADDING, 0);
                                        j = importedSprite.texture.height;
                                        break;
                                    }
                                }
                            }

                            spriteRects.Add(spriteRect);
                            spritePixels.Add(importedSprite.texture.GetPixels(
                                    (int)minValues.x,
                                    (int)minValues.y,
                                    (int)spriteRect.width,
                                    (int)spriteRect.height));
                        }
                        else
                        {
                            spriteRects.Add(spriteRect);
                            spritePixels.Add(importedSprite.texture.GetPixels());
                        }

                        spriteImporter.textureFormat = currentFormat;
                        spriteImporter.isReadable = false;
                        AssetDatabase.ImportAsset(atlasData.ImportedSpritePaths[i]);
                    }
                }

                EditorUtility.DisplayProgressBar("Create Atlas", string.Empty, 0.0f);

                Texture2D atlasTexture = new Texture2D(atlasData.Texture.width, atlasData.Texture.height, TextureFormat.ARGB32, false);

                Color32[] atlasPixels = atlasTexture.GetPixels32();
                for (int i = 0; i < atlasPixels.Length; i++)
                    atlasPixels[i] = new Color32(0, 0, 0, 0);
                atlasTexture.SetPixels32(atlasPixels);

                for (int i = 0; i < spriteRects.Count; i++)
                {
                    var rect = spriteRects[i];
                    var pixels = spritePixels[i];

                    atlasTexture.SetPixels(
                        (int)rect.x,
                        (int)rect.y,
                        (int)rect.width,
                        (int)rect.height,
                        pixels);
                }

                EditorUtility.DisplayProgressBar("Saving Atlas", string.Empty, 0.0f);

                atlasTexture.Apply();
                File.WriteAllBytes(Path.GetFullPath(atlasData.SavePath), atlasTexture.EncodeToPNG());

                EditorUtility.DisplayProgressBar("Refreshing Atlas", string.Empty, 0.0f);

                AssetDatabase.Refresh();

                TextureImporter textureImporter = AssetImporter.GetAtPath(atlasData.SavePath) as TextureImporter;

                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporter.mipmapEnabled = false;
                textureImporter.textureFormat = TextureImporterFormat.ARGB32;//TextureImporterFormat.RGBA16;
                textureImporter.anisoLevel = 0;
                textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.spritesheet = spriteMetaDatas.ToArray();
                textureImporter.isReadable = false;

                string[] savePathParts = Path.GetFileNameWithoutExtension(atlasData.SavePath).Split('@');
                textureImporter.assetBundleName = savePathParts[0];
                if (savePathParts.Length > 1)
                    textureImporter.assetBundleVariant = savePathParts[1];

                AssetDatabase.ImportAsset(atlasData.SavePath);

                CheckProccedTextures();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        #endregion
    }

    public class PackingTagInsertWindow : EditorWindow
    {
//        private string packingTag = string.Empty;
        private string savePath = "Assets/gui/";
		private int packingTagIndex = 0;

        private string errorMsg = string.Empty;

        private void OnGUI()
        {
            GUILayout.Label("Creation Settings", EditorStyles.boldLabel);

			string[] tags = {
				"new_gui_garage",
				"new_gui_common",
				"new_gui_garage_door",
				"new_gui_booster",
				"gui_reused",
				"gui_reused_2",
				"gui_garage",
				"gui_league",
				"gui_race",
				"gui_tex"
			};

//            packingTag = EditorGUILayout.TextField("Packing Tag", packingTag);
			packingTagIndex = EditorGUILayout.Popup(packingTagIndex, tags);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            
            GUILayout.Label(errorMsg, EditorStyles.boldLabel); 

            if (GUILayout.Button("create"))
            {
//                if (!string.IsNullOrEmpty(packingTag))
//                {
                if (!string.IsNullOrEmpty(savePath))
                {
				ImportSpritesAndCreateAtlas.CreateAtlasFromPackingTag(tags[packingTagIndex], savePath + tags[packingTagIndex]);
                    this.Close();
                }
                else
                    errorMsg = "Save path can not be empty!";
//                }
//                else
//                    errorMsg = "Packing tag can not be empty!";
            }
        }
    }
}
