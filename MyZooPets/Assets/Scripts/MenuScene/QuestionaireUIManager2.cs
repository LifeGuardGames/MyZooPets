using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Player age user interface manager.
/// </summary>
public class QuestionaireUIManager2 : SingletonUI<QuestionaireUIManager2> {
	private bool hasAsthma;
	private bool hasAsthmaOptionChecked = false;

	void Awake(){
		eModeType = UIModeTypes.CustomizePet;
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

	public void ButtonClickedFinish(){
		Analytics.Instance.UserAsthma(hasAsthma);
		DataManager.Instance.GameData.PetInfo.IsQuestionaireCollected = true; // TODO
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

	private IEnumerator ShowAgeSelector(){
		yield return new WaitForSeconds(1.5f);
		OpenUI();
	}
}
