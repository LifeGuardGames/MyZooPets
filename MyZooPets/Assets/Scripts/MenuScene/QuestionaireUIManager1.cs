using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireUIManager1 : MonoBehaviour{
	public UISlider slider;
	public UILabel label;
	public TweenToggle baseTweenToggle;
	public TweenToggle finishButtonTweenToggle;

	private int age;
	private bool hasMovedSlider = false;
	private GameObject menuScenePet = null;

	/// <summary>
	/// Called by the slider when the value has changed. Rounds up to the nearest integer for age
	/// </summary>
	public void OnSliderChange(){
		float percentage = slider.sliderValue;
		if(percentage == 0){
			return; 
		}
		else if(percentage == 1){
			age = 50;
			label.text = "50+";
		}
		else{
			age = Mathf.CeilToInt(percentage * 100f / 2f);	// Round up to the nearest 1-50 int
			label.text = age.ToString();
		}

		if(!hasMovedSlider){
			hasMovedSlider = true;
			finishButtonTweenToggle.Show();
		}
	}

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAge(age);
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
		QuestionaireManager.Instance.questionaireManager2.OpenUI();
	}
}
