using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireAgeController : MonoBehaviour{
	public TweenToggleDemux panelTween;
	public TweenToggle submitButtonTween;
	public Slider slider;
	public Text ageText;

	private int lastAgeAux;				// Keeping track of last age for sound effect
	private int age;
	private bool hasMovedSlider = false;

	/// <summary>
	/// Called by the slider when the value has changed. Rounds up to the nearest integer for age
	/// </summary>
	public void OnSliderChange(){
		int age = (int)slider.value;

		if(age == (int)slider.maxValue){
			ageText.text = age.ToString() + "+";
		}
		else{
			if(lastAgeAux != age){
				float pitchCount = 1f + (age * 0.1f);

				Hashtable hashOverride = new Hashtable();
				hashOverride["Pitch"] = pitchCount;
				AudioManager.Instance.PlayClip("buttonGeneric3", option: hashOverride);
				lastAgeAux = age;
			}

			ageText.text = age.ToString();
		}

		if(!hasMovedSlider){
			hasMovedSlider = true;
			submitButtonTween.Show();
		}
	}

	public void OnSubmitButton(){
		Analytics.Instance.UserAge(age);
		QuestionaireManager.Instance.QuestionaireCollected();	// Called again later, just to be safe
		HidePanel();
	}

	public void ShowPanel(){
		panelTween.Show();
	}

	public void HidePanel(){
		panelTween.Hide();
	}

	// Assigned callback
	public void OnFinishClose(){
		QuestionaireManager.Instance.questionaireAsthmaController.ShowPanel();
	}
}
