using UnityEngine;
using System.Collections;

// TODO incomplete! please finish conditionals after logic and data is in place
public class ButtonSetHighlightLanguages : ButtonSetHighlight {

	protected override void _Start(){
		string currentLanguage = SettingsUIManager.Instance.GetLocalization();

		if(currentLanguage == "English"){
			gameObject.transform.position = buttonList[0].transform.position;
		}
		else if(currentLanguage == "Spanish"){
			gameObject.transform.position = buttonList[1].transform.position;
		}
		else{
			Debug.LogError("Error in getting language");
		}
	}
}
