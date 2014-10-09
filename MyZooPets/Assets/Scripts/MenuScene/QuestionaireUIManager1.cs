using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireUIManager1 : SingletonUI<QuestionaireUIManager1> {

	public UISlider slider;
	public UILabel label;
	public TweenToggle finishButtonTweenToggle;

	private int age;
	private bool hasMovedSlider = false;

	void Awake(){
		eModeType = UIModeTypes.CustomizePet;
	}
	
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
		}
	}

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAge(age);
		DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected = true;

		CloseUI();
	}

	protected override void _OpenUI(){
		gameObject.GetComponent<TweenToggle>().Show();

	}

	protected override void _CloseUI(){
		gameObject.GetComponent<TweenToggle>().Hide();

		//once questionaire is done. let tutorial knows to continue
		TutorialManagerBedroom tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManagerBedroom>();
		tutorialManager.OnQuestionaireDone();
	}

	/// <summary>
	/// Callback for finish tweening
	/// </summary>
	public void DestroyPanel(){
		Destroy(gameObject);
	}

	public void ButtonPrivacyPolicy(){
		// TODO Add link here
	}

	public void ButtonTermsOfService(){
		// TODO Add link here
	}

	private IEnumerator ShowAgeSelector(){
		yield return new WaitForSeconds(1.5f);
		OpenUI();
	}
}
