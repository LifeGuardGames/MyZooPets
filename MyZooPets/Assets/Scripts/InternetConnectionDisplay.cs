using UnityEngine;
using System.Collections;

public class InternetConnectionDisplay : MonoBehaviour {

	public RotateAroundCenter loadingIconSpin;
	public UILocalize labelLocalize;
	
	public void Play(string labelLocalizeKey){
		loadingIconSpin.gameObject.SetActive(true);
		loadingIconSpin.GetComponent<ScaleTweenToggle>().Show();
		loadingIconSpin.Play();
		labelLocalize.key = labelLocalizeKey;
		labelLocalize.Localize();
	}

	public void Stop(string labelLocalizeKey){
		loadingIconSpin.GetComponent<ScaleTweenToggle>().Hide();
		labelLocalize.key = labelLocalizeKey;
		labelLocalize.Localize();
	}

	// Callback from hide tween
	public void DeactivateSpinSprite(){
		loadingIconSpin.gameObject.SetActive(false);
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Open")){
//			loadingIconSpin.Play();
//			OpenUI();
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "Close")){
//			loadingIconSpin.Stop();
//			CloseUI();
//		}
//	}
}
