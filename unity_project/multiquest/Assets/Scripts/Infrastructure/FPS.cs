using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
	float deltaTime = 0.0f;

	public static FPS instance;
	public Color textCol;

	void Awake()
	{
		if(instance != null)
			GameObject.Destroy(this.gameObject);
		else
			instance = this;
	}

	void Update()
	{
		deltaTime += (Time.smoothDeltaTime - deltaTime) * 0.1f;
	}
	
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = textCol;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}