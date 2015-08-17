using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireUIManager2 : MonoBehaviour{
	public TweenToggle baseTweenToggle;
	public TweenToggle finishButtonTweenToggle;

	private bool hasAsthma;
	private bool hasAsthmaOptionChecked = false;

	/// <summary>
	/// Event callback when yes radio button is clicked
	/// </summary>
	public void OnAsthmaYes(bool isChecked){
		if(isChecked){
			AudioManager.Instance.PlayClip("buttonGeneric3");
			hasAsthma = true;
			if(!hasAsthmaOptionChecked){
				hasAsthmaOptionChecked = true;
				finishButtonTweenToggle.Show();
			}
		}
	}

	/// <summary>
	/// Event callback when no radio button is clicked
	/// </summary>
	public void OnAsthmaNo(bool isChecked){
		if(isChecked){
			AudioManager.Instance.PlayClip("buttonGeneric3");
			hasAsthma = false;
			if(!hasAsthmaOptionChecked){
				hasAsthmaOptionChecked = true;
				finishButtonTweenToggle.Show();
			}
		}
	}

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAsthma(hasAsthma);
		QuestionaireManager.Instance.QuestionaireCollected();
		CloseUI();
	}

	public void OpenUI(){
		baseTweenToggle.Show();
	}

	public void CloseUI(){
		baseTweenToggle.Hide();
	}

	// Assigned callback
	public void FinishedCloseFunction(){
		QuestionaireManager.Instance.ContinueLoading();
	}
}
