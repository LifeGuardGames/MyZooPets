using UnityEngine;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireAsthmaController : MonoBehaviour{
	public TweenToggleDemux panelTween;
	public TweenToggle submitButtonTween;

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
				submitButtonTween.Show();
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
				submitButtonTween.Show();
			}
		}
	}

	public void OnSubmitButton(){
		Analytics.Instance.UserAsthma(hasAsthma);
		QuestionaireManager.Instance.QuestionaireCollected();
		HidePanel();
	}

	public void ShowPanel() {
		panelTween.Show();
	}

	public void HidePanel() {
		panelTween.Hide();
	}

	// Assigned callback
	public void OnFinishClose() {
		QuestionaireManager.Instance.ContinueLoading(true);
	}
}
