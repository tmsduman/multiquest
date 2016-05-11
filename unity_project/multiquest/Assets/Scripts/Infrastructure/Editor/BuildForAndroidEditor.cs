// C# example.
using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;

public class BuildForAndroidEditor
{
	[MenuItem("Tools/Android Build With Postprocess")]
	public static void BuildGame ()
	{
		// Get filename.
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
		List<string> levels = new List<string> (); 

		var sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();
		foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes) {
			var scene = EditorSceneManager.OpenScene (editorScene.path);
			levels.Add ("Assets/Scenes/" + scene.name + ".unity");

		}
		EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);


		//move assets away from streaming folder

		FileUtil.ReplaceDirectory ("Assets/StreamingAssets/Export", "Assets/ExportedAssets");
		FileUtil.DeleteFileOrDirectory ("Assets/StreamingAssets/Export");

		// Build player.
		BuildPipeline.BuildPlayer(levels.ToArray (), path + "/SpeedIslandsPrototype.apk", BuildTarget.Android, BuildOptions.None);

		//move assets back to streaming folder
		FileUtil.ReplaceDirectory ("Assets/ExportedAssets", "Assets/StreamingAssets/Export");
		FileUtil.DeleteFileOrDirectory ("Assets/ExportedAssets");


//		// Copy a file from the project folder to the build folder, alongside the built game.
//		FileUtil.CopyFileOrDirectory("Assets/WebPlayerTemplates/Readme.txt", path + "Readme.txt");
//
//		// Run the game (Process class from System.Diagnostics).
//		Process proc = new Process();
//		proc.StartInfo.FileName = path + "BuiltGame.exe";
//		proc.Start();
	}
}