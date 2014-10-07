using UnityEngine;
using System.Collections;

public class InternetConnectionDisplay : MonoBehaviour {

	public RotateAroundCenter loadingIconSpin;
	public GameObject labelTweenParent;
	public UILocalize labelLocalize;
	public Color normalTextColor;
	public Color errorTextColor;
	
	public void Play(string labelLocalizeKey){
		loadingIconSpin.gameObject.SetActive(true);
		loadingIconSpin.GetComponent<ScaleTweenToggle>().Show();
		loadingIconSpin.Play();
		labelTweenParent.GetComponent<ScaleTweenToggle>().Show();
		labelLocalize.GetComponent<UILabel>().color = normalTextColor;
		labelLocalize.key = labelLocalizeKey;
		labelLocalize.Localize();
	}

	public void Stop(bool isSuccess, string labelLocalizeKey){
		Debug.Log("Stopping spinner");
		loadingIconSpin.GetComponent<ScaleTweenToggle>().Hide();

		if(isSuccess){
			labelTweenParent.GetComponent<ScaleTweenToggle>().Hide();
			Debug.Log("success, hiding label");
		}
		else{
			// Dont hide it and show error message
			labelLocalize.GetComponent<UILabel>().color = errorTextColor;
			labelLocalize.key = labelLocalizeKey;
			labelLocalize.Localize();
		}
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
