using UnityEngine;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class PlayerAgeUIManager : SingletonUI<PlayerAgeUIManager> {

	public UISlider slider;
	public UILabel label;
	private int age;

	/// <summary>
	/// Called by the slider when the value has changed. Rounds up to the nearest integer for age
	/// </summary>
	public void OnSliderChange(){
		float percentage = slider.sliderValue;
		if(percentage == 1){
			age = 50;
			label.text = "50+";
		}
		else{
			age = Mathf.CeilToInt(percentage * 100f / 2f);	// Round up to the nearest 1-50 int
			label.text = age.ToString();
		}
	}

	protected override void _OpenUI(){
		gameObject.GetComponent<TweenToggleDemux>().Show();
	}

	protected override void _CloseUI(){
		gameObject.GetComponent<TweenToggleDemux>().Hide();
	}
}
