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

//		bool isInFirstPartition = LoadIntoFirstPartition();

//		if(!isInFirstPartition){
			if(sceneData != null)
				if(sceneData.LastScene == Application.loadedLevelName)
					transform.position = sceneData.LastPetPosition;
//		}
	}

//	void OnApplicationPause(bool paused){
//		if(!paused){
//			LoadIntoFirstPartition();
//		}
//	}
//
//	/// <summary>
//	/// Loads the into first partition. 
//	/// </summary>
//	/// <returns><c>true</c>, if into first partition was loaded, <c>false</c> otherwise.</returns>
//	private bool LoadIntoFirstPartition(){
//		bool retVal = false;
//
//		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME);
//		bool isTriggerTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_TRIGGERS);
//		bool isDecoTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_DECOS);
//		
//		if(isFlameTutorialDone && (!isFlameTutorialDone || !isDecoTutorialDone)){
//			//if the 2nd part of the tutorial is not done yet spawn the pet back in the first partition
////			transform.position = new Vector3(10f, 0f, 24f);
//			CameraManager.Instance.GetPanScript().MoveOneRoomToLeft();
//			retVal = true;
//		}
//
//		return retVal;
//	}
}
