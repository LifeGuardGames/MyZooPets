using UnityEngine;
using System.Collections;

public class InternetConnectionDisplay : MonoBehaviour {

	public RotateAroundCenter loadingIconSpin;
	


	public void Show(){
		GetComponent<TweenToggle>().Show();
	}

	public void Hide(){
		GetComponent<TweenToggle>().Hide();
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
