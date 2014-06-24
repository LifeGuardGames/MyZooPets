using UnityEditor;
using UnityEngine;

public class MoveVariableData : EditorWindow {
	[MenuItem("Window/MoveVariableData")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(MoveVariableData));
	}
	
	void OnGUI() {
		if(GUILayout.Button("MoveData")) {
			LgButton[] components = Resources.FindObjectsOfTypeAll<LgButton>();
			foreach (LgButton component in components) {
				if(!component.modeTypes.Contains(component.eMode))
					component.modeTypes.Add(component.eMode);

				component.isSprite = component.bSprite;
				component.isCheckingClickManager = component.bCheckClickManager;
				component.buttonSound = component.strSoundProcess;
			}
		}
	}
}

