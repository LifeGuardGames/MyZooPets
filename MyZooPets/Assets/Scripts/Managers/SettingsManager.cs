using UnityEngine;
using System.Collections;

public class SettingsManager : Singleton<SettingsManager> {

	public string GetCurrentLanguage(){
		return Localization.instance.currentLanguage;
	}

	public void SetCurrentLanguage(string language){
		Localization.instance.currentLanguage = language;
	}
}
