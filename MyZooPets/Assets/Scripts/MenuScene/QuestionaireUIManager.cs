using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireUIManager : SingletonUI<QuestionaireUIManager> {

	public UISlider slider;
	public UILabel label;
	public TweenToggle finishButtonTweenToggle;

	private int age;
	private bool hasMovedSlider = false;

	private bool hasAsthma;
	private bool hasAsthmaOptionChecked = false;

	void Awake(){
		eModeType = UIModeTypes.CustomizePet;
	}

	void Start(){

		DateTime nextPlayPeriod = PlayPeriodLogic.Instance.NextPlayPeriod;
		bool isFlameTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME);
		bool isQuestionaireCollected = DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected;

		//this questionaire should only come up if 2nd play period and the flame tutorial
		//has been finished
		if(LgDateTime.GetTimeNow() >= nextPlayPeriod && !isQuestionaireCollected &&
		   isFlameTutorialDone){
			Invoke("OpenUI", 1f);
		}
	}

	private IEnumerator ShowAgeSelector(){
		yield return new WaitForSeconds(1.5f);
		OpenUI();
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

	/// <summary>
	/// Event callback when yes radio button is clicked
	/// </summary>
	public void OnAsthmaYes(bool isChecked){
		if(isChecked){
			hasAsthma = true;
			hasAsthmaOptionChecked = true;
		}
	}

	/// <summary>
	/// Event callback when no radio button is clicked
	/// </summary>
	public void OnAsthmaNo(bool isChecked){
		if(isChecked){
			hasAsthma = false;
			hasAsthmaOptionChecked = true;
		}
	}

	void Update(){
		if(hasMovedSlider && hasAsthmaOptionChecked){
			finishButtonTweenToggle.Show();
		}
	}

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAge(age);
		Analytics.Instance.UserAsthma(hasAsthma);
		DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected = true;

		CloseUI();
	}

	protected override void _OpenUI(){
		gameObject.GetComponent<TweenToggle>().Show();
	}

	protected override void _CloseUI(){
		gameObject.GetComponent<TweenToggle>().Hide();
	}

	/// <summary>
	/// Callback for finish tweening
	/// </summary>
	public void DestroyPanel(){
		Destroy(gameObject);
	}
}
