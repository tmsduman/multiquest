using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor.SceneManagement;


public class UnusedSpriteWindow : EditorWindow
{
	int index = 0;
//	string packingTags = "";
	List<string> sceneNames = new List<string> ();


	private void OnGUI()
	{
		GUILayout.Label ("Lists sprites not in atlasses scene by scene");
		GUILayout.Space (20);
		GUILayout.Label ("Select Scene:");
		if (sceneNames.Count == 0) {
			var sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();
			sceneNames = new List<string> ();
			foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes) {
				var scene = EditorSceneManager.OpenScene (editorScene.path);
				sceneNames.Add (scene.name);

			}
			EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);
		}
		index = EditorGUILayout.Popup(index, sceneNames.ToArray ());

//		GUILayout.Label ("Packing Tags (comma separated, leave empty if all):");
//		packingTags = EditorGUILayout.TextField (packingTags);
		this.Repaint ();
		if (GUILayout.Button ("show")) {
			RemoveUnusedGraphics.ListSprites (index);
		}
	}
}

public class RemoveUnusedGraphics
{

	[MenuItem("Tools/List Sprite References")]
	public static void OpenUnusedSpriteWindow()
	{
		UnusedSpriteWindow window =
			(UnusedSpriteWindow)EditorWindow.GetWindow(
				typeof(UnusedSpriteWindow),
				false,
				"List sprites",
				true);
		window.Show();
	}



	public static void ListSprites (int sceneIndex) {
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			return;
		
		var sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();

		EditorSceneManager.OpenScene(EditorBuildSettings.scenes[sceneIndex].path);

		foreach (Sprite sprite in Resources.FindObjectsOfTypeAll <Sprite> ()) {
			Object[] objectArray = EditorUtility.CollectDependencies (new Object[] { sprite });
			if (sprite.name == sprite.texture.name) {
//				if (sprite.name == sprite.texture.name && 0 != string.Compare (AssetDatabase.GetAssetPath (sprite), "Resources/unity_builtin_extra")) {
				Debug.Log (AssetDatabase.GetAssetPath (sprite) + " : " + sprite.texture.name );
				foreach (Object o in objectArray) {
					FindReferencesTo (o);
				}
			}
		}

		EditorUtility.ClearProgressBar();

		EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);
	}

	private static void FindReferencesTo(Object to)
	{
		var referencedBy = new List<Object>();
		var allObjects = Resources.FindObjectsOfTypeAll<GameObject> ();
		for (int j = 0; j < allObjects.Length; j++)
		{
			var go = allObjects[j];

//			if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
//			{
//				if (PrefabUtility.GetPrefabParent(go) == to)
//				{
//					Debug.Log(string.Format("\t(PREFAB) REFERENCED BY: {0}\n\t COMPONENT: {1}", GetHierarchy (go.transform, string.Empty), go.GetType()), go);
//					referencedBy.Add(go);
//				}
//			}

			var components = go.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				var c = components[i];
				if (!c) continue;

				var so = new SerializedObject(c);
				var sp = so.GetIterator();

				while (sp.Next(true))
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if (sp.objectReferenceValue == to)
						{
							Debug.Log(string.Format("\tREFERENCED BY: {0}\n\t COMPONENT: {1}", GetHierarchy (c.transform, string.Empty), c.GetType()), c);
							referencedBy.Add(c.gameObject);
						}
					}
			}
		}
	}

	static string GetHierarchy (Transform t, string hierarchy) {
		if (t.parent == null) {
			return t.name + "/" + hierarchy;
		}

		return GetHierarchy (t.parent, t.name + "/" + hierarchy);
	}


    [MenuItem("Tools/Remove unused graphics")]
    public static void Remove()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();

        // Get all sprites in project
        string[] assetGUIDs = AssetDatabase.FindAssets("t:Sprite");
        List<string> spritesAssetPaths = new List<string>(assetGUIDs.Length);
        foreach (var item in assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(item);
            if(!spritesAssetPaths.Contains(path))
                spritesAssetPaths.Add(path);
        }

        // Check all scenes
        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene");
        
        int sceneCount = 0;
        foreach (var item in sceneGUIDs)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(item);
            Scene scene = EditorSceneManager.OpenScene(scenePath);
            EditorUtility.DisplayProgressBar("Check scenes", "Proceed scene " + scene.name, sceneCount / sceneGUIDs.Length);
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                // Images
                foreach (var image in rootGameObject.GetComponentsInChildren<Image>(true))
                {
                    if (image.sprite != null)
                    {
                        string spriteAssetPath = AssetDatabase.GetAssetPath(image.sprite);
                        if (spritesAssetPaths.Contains(spriteAssetPath))
                            spritesAssetPaths.Remove(spriteAssetPath);
                    }
                }

                // SpriteRenderer
                foreach (var spriteRenderer in rootGameObject.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (spriteRenderer.sprite != null)
                    {
                        string spriteAssetPath = AssetDatabase.GetAssetPath(spriteRenderer.sprite);
                        if (spritesAssetPaths.Contains(spriteAssetPath))
                            spritesAssetPaths.Remove(spriteAssetPath);
                    }
                }
            }
            sceneCount++;
        }

        EditorUtility.ClearProgressBar();

        EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);

        // Check all prefabs
        string[] prefabsGUIDs = AssetDatabase.FindAssets("t:Prefab");
        int prefabCount = 0;
        foreach (var item in prefabsGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(item);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            EditorUtility.DisplayProgressBar("Check prefabs", "Proceed prefab " + prefab.name, prefabCount / prefabsGUIDs.Length);

            // Image
            foreach (var image in prefab.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite != null)
                {
                    string spriteAssetPath = AssetDatabase.GetAssetPath(image.sprite);
                    if (spritesAssetPaths.Contains(spriteAssetPath))
                        spritesAssetPaths.Remove(spriteAssetPath);
                }
            }

            // SpriteRenderer
            foreach (var spriteRenderer in prefab.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (spriteRenderer.sprite != null)
                {
                    string spriteAssetPath = AssetDatabase.GetAssetPath(spriteRenderer.sprite);
                    if (spritesAssetPaths.Contains(spriteAssetPath))
                        spritesAssetPaths.Remove(spriteAssetPath);
                }
            }

            prefabCount++;
        }

        EditorUtility.ClearProgressBar();

        // Check all materials
        string[] materialsGUIDs = AssetDatabase.FindAssets("t:Material");
        int materialCount = 0;
        foreach (var item in materialsGUIDs)
        {
            string materialPath = AssetDatabase.GUIDToAssetPath(item);
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            EditorUtility.DisplayProgressBar("Check materials", "Proceed material " + material.name, materialCount / materialsGUIDs.Length);

            if (material.mainTexture != null)
            {
                string textureAssetPath = AssetDatabase.GetAssetPath(material.mainTexture);
                if (spritesAssetPaths.Contains(textureAssetPath))
                    spritesAssetPaths.Remove(textureAssetPath);
            }
            materialCount++;
        }

        EditorUtility.ClearProgressBar();

        if (spritesAssetPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("Remove unused graphics", "Found no unused graphics", "yippie");
        }
        else
        {
            if (EditorUtility.DisplayDialog("Remove unused graphics", "Found " + spritesAssetPaths.Count + " unused graphics.", "Delete them", "Just print pathes to console"))
            {
                for (int i = 0; i < spritesAssetPaths.Count; i++)
                {
                    AssetDatabase.MoveAssetToTrash(spritesAssetPaths[i]);
                }
            }
            else
            {
                for (int i = 0; i < spritesAssetPaths.Count; i++)
                {
                    Debug.Log("unused graphic " + spritesAssetPaths[i]);
                }
            }
        }
    }
}
