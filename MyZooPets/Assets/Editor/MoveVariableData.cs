using UnityEditor;
using UnityEngine;

public class MoveVariableData : EditorWindow {
	[MenuItem("Window/MoveVariableData")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(MoveVariableData));
	}
	
	void OnGUI() {
		if(GUILayout.Button("MoveData")) {
//			AudioManager[] components = Resources.FindObjectsOfTypeAll<AudioManager>();
//			foreach (AudioManager component in components) {
//
//				component.backgroundMusic = component.strBgMusic;
//
//				EditorUtility.SetDirty(component.gameObject);
//			}
		}
	}
}

