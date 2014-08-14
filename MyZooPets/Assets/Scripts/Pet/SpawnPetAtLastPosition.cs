using UnityEngine;
using System.Collections;

//---------------------------------------------------
// SpawnAtLastPosition
// If pet's position was saved from the last scene
// then spawn pet back at that position
//---------------------------------------------------
public class SpawnPetAtLastPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LoadSceneData sceneData = DataManager.Instance.SceneData;
        if(sceneData != null)
            if(sceneData.LastScene == Application.loadedLevelName)
                transform.position = sceneData.LastPetPosition;
	}
}
