using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Questionaire manager. Check when it's appropriate to spawn the questionaire
/// </summary>
public class QuestionaireManager : MonoBehaviour {
	void Start(){
		CheckToOpenQuestionaire();
	}
	
	void OnApplicationPause(bool paused){
		if(!paused){
			CheckToOpenQuestionaire();
		}
	}

	private void CheckToOpenQuestionaire(){
		DateTime nextPlayPeriod = PlayPeriodLogic.Instance.NextPlayPeriod;
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME);
		bool isQuestionaireCollected = DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected;
		
		//this questionaire should only come up if in 2nd play period and the flame tutorial
		//has been finished
		if(LgDateTime.GetTimeNow() >= nextPlayPeriod && !isQuestionaireCollected &&
		   isFlameTutorialDone){
			GameObject questionaireUIPrefab = (GameObject) Resources.Load("QuestionairePanel");
			LgNGUITools.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), questionaireUIPrefab);
	
			Invoke("ShowQuestionaire", 1f);
		}
	}

	private void ShowQuestionaire(){
		QuestionaireUIManager.Instance.OpenUI();
	}
}
