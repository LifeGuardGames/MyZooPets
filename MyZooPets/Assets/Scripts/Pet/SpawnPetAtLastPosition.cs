using UnityEngine;
using System.Collections;

/// <summary>
/// If pet's position was saved from the last scene
/// then spawn pet back at that position
/// </summary>
public class SpawnPetAtLastPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LoadSceneData sceneData = DataManager.Instance.SceneData;

		if(sceneData != null) {
			if(sceneData.LastScene == SceneUtils.CurrentScene) {
				transform.position = sceneData.LastPetPosition;
			}
		}
	}
}
