using System;
using UnityEditor;
using UnityEngine;
using Infrastructure.AssetBundleVersionManager;

[CustomEditor (typeof(AssetBundleVersionManager))]
public class VersionManagerEditor : Editor
{
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();


		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Clean Cache")) 
		{
			Caching.CleanCache ();
			Debug.Log ("Called Caching.CleanCache ()");
		}
	}
}

