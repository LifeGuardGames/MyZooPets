using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
[ExecuteInEditMode]
public class UIHelper : EditorWindow {
	private GameObject[] tagsList;
	private string currentScene = "";
	private GameObject UIElementListObject; // Object in the scene that keeps of all UIElements
	private List<GameObject> UIElementsList;
	private List<bool> elementBoolList;
	private List<bool> savedElementBoolList;
	private Vector2 scrollPosition = Vector2.zero;

	[MenuItem("LGG/UI Helper")]
	public static void ShowWindow() {
		GetWindow(typeof(UIHelper));
	}

	public void Reload() {
		// Attempt to load
		currentScene = SceneManager.GetActiveScene().name;
		tagsList = GameObject.FindGameObjectsWithTag("UIElementList");
		if(tagsList.Length > 0) {
			UIElementsList = GameObject.FindGameObjectsWithTag("UIElementList")[0].GetComponent<UIHelperList>().UIElementList;
			elementBoolList = GameObject.FindGameObjectsWithTag("UIElementList")[0].GetComponent<UIHelperList>().defaultBools;
		}
	}

	public void OnHierarchyChange() {
		if(currentScene != SceneManager.GetActiveScene().name) {
			Reload();
		}
	}

	void OnGUI() {
		if(tagsList != null && tagsList.Length > 0) {
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
			if(GUILayout.Button("Refresh Helper", GUILayout.Height(50))) {
				Reload();
			}
			GUILayout.Label("Toggle UI Elements", EditorStyles.boldLabel);
			if(GUILayout.Button("Disable All")) {
				DisableUIElements();
			}
			for(int i = 0; i < UIElementsList.Count; i++) {
				if(UIElementsList[i] != null) { // Some elements are persistent-destroyed
					GUILayout.BeginHorizontal();
					GUILayout.Toggle(elementBoolList[i], "");
					GUILayout.Label(UIElementsList[i].name);
					if(GUILayout.Button("Solo", GUILayout.Width(50))) {
						SoloUI(UIElementsList[i]);
					}
					GUILayout.EndHorizontal();
				}
			}

			if(GUILayout.Button("Reset", GUILayout.Height(50))) {
				ResetUIElements();
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
			GUILayout.EndScrollView();
		}
	}

	// Singles out the UI element passed into the list
	private void SoloUI(GameObject soloGo) {
		foreach(GameObject go in UIElementsList) {
			go.SetActive(go == soloGo ? true : false);
		}
	}

	// Disables all the UI elements
	private void DisableUIElements() {
		foreach(GameObject go in UIElementsList) {
			go.SetActive(false);
		}
	}

	// Go through the UI list and reset them to its original enable state
	private void ResetUIElements() {
		for(int i = 0; i < UIElementsList.Count; i++) {
			UIElementsList[i].SetActive(elementBoolList[i]);
		}
	}
}
