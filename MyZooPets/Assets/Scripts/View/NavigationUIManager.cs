using UnityEngine;
using System.Collections;

public class NavigationUIManager : Singleton<NavigationUIManager> {
	public GameObject navigationPanel;
	
	// used with tutorials
	public GameObject decoButton;
	public GameObject DecoButton{
		get{ return decoButton; }
	}

	public GameObject decoButtonShine;

	public void ToggleShineDecoButton(bool isOn){
		decoButtonShine.SetActive(isOn);
	}

	public void ShowPanel(){
		navigationPanel.GetComponent<TweenToggleDemux>().Show();
	}

	public void HidePanel(){
		navigationPanel.GetComponent<TweenToggleDemux>().Hide();
	}
}
