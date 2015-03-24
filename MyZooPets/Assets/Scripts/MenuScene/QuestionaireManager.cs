using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Questionaire manager. Check when it's appropriate to spawn the questionaire
/// </summary>
public class QuestionaireManager : Singleton<QuestionaireManager> {
	public QuestionaireUIManager1 questionaireManager1;
	public QuestionaireUIManager2 questionaireManager2;

	void Start(){
		CheckToOpenQuestionaire();
	}

	/// <summary>
	/// Checks to open questionaire in loading scene. Collects user age and asthma info
	/// </summary>
	private void CheckToOpenQuestionaire(){
		if(!DataManager.Instance.IsQuestionaireCollected){
			Debug.Log("Not Collected");
			Invoke("ShowQuestionaire", 0.5f);
		}
		else{
			Debug.Log("Collected");
			ContinueLoading();
		}
	}

	private void ShowQuestionaire(){
		questionaireManager1.OpenUI();
	}

	/// <summary>
	/// Collected information about player
	/// </summary>
	public void QuestionaireCollected(){
		if (Debug.isDebugBuild){
			return;
		}
		DataManager.Instance.IsQuestionaireCollected = true;
	}

	public void ContinueLoading(){
		if(DataManager.Instance.GameData.PetInfo.IsHatched){
			Application.LoadLevel(SceneUtils.BEDROOM);
		}
		else{
			Application.LoadLevel(SceneUtils.MENU);
		}
	}
}
