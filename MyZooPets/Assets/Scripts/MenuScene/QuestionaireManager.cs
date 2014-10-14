using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Questionaire manager. Check when it's appropriate to spawn the questionaire
/// </summary>
public class QuestionaireManager : Singleton<QuestionaireManager> {
	void Start(){
		if(Application.loadedLevelName == "MenuScene"){
			CheckToOpenQuestionaireMenuScene();
		}
		else if(Application.loadedLevelName == "NewBedRoom"){
			CheckToOpenQuestionaireBedroom();
		}
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused && Application.loadedLevelName == "NewBedRoom"){
			CheckToOpenQuestionaireBedroom();
		}
	}

	/// <summary>
	/// Accepts the terms and privacy.
	/// </summary>
	public void AcceptTermsAndPrivacy(){
		DataManager.Instance.IsTermsAndPrivacyAccepeted = true;
	}

	/// <summary>
	/// Asthma info collected.
	/// </summary>
	public void AsthmaInfoCollected(){
		DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected = true;
	}

	/// <summary>
	/// Checks to open questionaire menu scene. Collects user age and agreement to
	/// privacy policy and terms of agreement
	/// </summary>
	private void CheckToOpenQuestionaireMenuScene(){
		bool isQuestionaireCollected = DataManager.Instance.IsTermsAndPrivacyAccepeted;

		if(!isQuestionaireCollected){
			GameObject questionaireUIPrefab = (GameObject) Resources.Load("QuestionairePanel1");
			LgNGUITools.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), questionaireUIPrefab);

			Invoke("ShowQuestionaireMenuScene", 1f);
		}
	}

	/// <summary>
	/// Checks to open questionaire bedroom. This questionaire collects asthma info
	/// </summary>
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
