using UnityEngine;
using System.Collections;

public class RoomGui : MonoBehaviour {
	
	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	
	void Start (){
	
	}
	
	void Update (){
	
	}
	
	void OnGUI(){
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
		
		GUI.Box(new Rect(0, 0, 500, 50), "Tier level");
		GUI.Box(new Rect(510, 0, 200, 50), "Stars x " + 1);
		GUI.Box(new Rect(0, 70, 90, 90), "Health");
		GUI.Box(new Rect(0, 170, 90, 90), "Hunger");
		GUI.Box(new Rect(0, 270, 90, 90), "Mood");
		
		GUILayout.BeginArea(new Rect(0, NATIVE_HEIGHT - 100, 475, 100), "test");
		GUILayout.BeginHorizontal("box");
		GUILayout.Button("+", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("food", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("Inhaler", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("explore", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("EmInhaler", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
