using UnityEngine;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class PlayerAgeUIManager : SingletonUI<PlayerAgeUIManager> {

	public UISlider slider;
	public UILabel label;
	public TweenToggle finishButtonTweenToggle;
	public ParticleSystemController leafParticle;

	private int age;
	private bool hasMovedSlider = false;

	void Awake(){
		eModeType = UIModeTypes.CustomizePet;
	}

	void Start(){
		if(SelectionManager.Instance.IsFirstTime)
			StartCoroutine(ShowAgeSelector());
//			Invoke("OpenUI", f);
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
			finishButtonTweenToggle.Show();
			hasMovedSlider = true;
		}
	}

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAge(age);
		CloseUI();
	}

	protected override void _OpenUI(){
		leafParticle.Stop();
		gameObject.GetComponent<TweenToggle>().Show();
	}

	protected override void _CloseUI(){
		leafParticle.Play();
		gameObject.GetComponent<TweenToggle>().Hide();
	}
}
