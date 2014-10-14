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
	private GameObject menuScenePet = null;

	void Awake(){
		eModeType = UIModeTypes.CustomizePet;
	}

	protected override void Start(){
		base.Start();

		//pet sprite need to be disable if it's in the scene because of layering issue
		menuScenePet = GameObject.Find("MenuScenePet");
		if(menuScenePet != null)
			menuScenePet.SetActive(false);
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
			finishButtonTweenToggle.Show();
		}
	}

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAge(age);
		QuestionaireManager.Instance.AcceptTermsAndPrivacy();

		CloseUI();
	}

	protected override void _OpenUI(){

	}

	protected override void _CloseUI(){
		DestroyPanel();
		if(menuScenePet != null)
			menuScenePet.SetActive(true);
	}

	/// <summary>
	/// Callback for finish tweening
	/// </summary>
	public void DestroyPanel(){
		Destroy(gameObject);
	}

	public void ButtonPrivacyPolicy(){
		Application.OpenURL("http://www.wellapets.com/privacy");
	}

	public void ButtonTermsOfService(){
		Application.OpenURL("http://www.wellapets.com/terms");
	}

	private IEnumerator ShowAgeSelector(){
		yield return new WaitForSeconds(1.5f);
		OpenUI();
	}
}
