using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CurrentSceneInfo : EditorWindow {
	[MenuItem("LGG/Current Scene Info")]
	public static void ShowWindow() {
		GetWindow(typeof(CurrentSceneInfo));
	}

	void OnGUI() {
		if(Application.isPlaying) {
			if(SceneManager.GetActiveScene().name != SceneUtils.BEDROOM || SceneManager.GetActiveScene().name != SceneUtils.YARD) {
				GUILayout.Label("ClickManager Properties", EditorStyles.boldLabel);
				EditorGUILayout.TextField("Stack Size", ClickManager.Instance.StackModes.Count.ToString());
				EditorGUILayout.TextField("Current Mode", ClickManager.Instance.CurrentMode.ToString());

				GUILayout.Label("Notification Properties", EditorStyles.boldLabel);
				EditorGUILayout.TextField("Queue Size", NotificationQueueData.QueueCount().ToString());

				GUILayout.Label("Pet Information", EditorStyles.boldLabel);
				EditorGUILayout.Toggle("Can blow fire", DataManager.Instance.GameData.PetInfo.CanBreathFire());
			}
			// Add any other things that you want to see here
		}
	}

	public void OnInspectorUpdate() {
		// This will only get called 10 times per second.
		Repaint();
	}
}
