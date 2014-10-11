using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Questionaire manager. Check when it's appropriate to spawn the questionaire
/// </summary>
public class QuestionaireManager : MonoBehaviour {
	void Start(){
		if(Application.loadedLevelName == "MenuScene"){
			CheckToOpenQuestionaireMenuScene();
		}
		else if(Application.loadedLevelName == "NewBedRoom"){
			CheckToOpenQuestionaireBedroom();
		}
	}

	private void CheckToOpenQuestionaireMenuScene(){
		// TODO JASON FIX HERE
		bool isQuestionaireCollectedMenuScene = DataManager.Instance.GameData.PetInfo.IsQuestionaireCollectedMenuScene;
		/////////

		if(isQuestionaireCollectedMenuScene){
			GameObject questionaireUIPrefab = (GameObject) Resources.Load("QuestionairePanel2");
			LgNGUITools.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), questionaireUIPrefab);

			Invoke("ShowQuestionaireMenuScene", 1f);
		}
	}

	private void CheckToOpenQuestionaireBedroom(){
		DateTime nextPlayPeriod = PlayPeriodLogic.Instance.NextPlayPeriod;
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME);
		bool isQuestionaireCollected = DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected;
		
		//this questionaire should only come up if in 2nd play period and the flame tutorial
		//has been finished
		if(LgDateTime.GetTimeNow() >= nextPlayPeriod && !isQuestionaireCollected &&
		   isFlameTutorialDone){
			GameObject questionaireUIPrefab = (GameObject) Resources.Load("QuestionairePanel2");
			LgNGUITools.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), questionaireUIPrefab);
	
			Invoke("ShowQuestionaireBedroom", 1f);
		}
	}

	private void ShowQuestionaireBedroom(){
		QuestionaireUIManager2.Instance.OpenUI();
	}

	private void ShowQuestionaireMenuScene(){
		QuestionaireUIManager1.Instance.OpenUI();
	}
}
