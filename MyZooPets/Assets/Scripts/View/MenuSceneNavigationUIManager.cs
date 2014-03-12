using UnityEngine;
using System.Collections;

// Dont want to use SingletonUI because no mode lock
public class MenuSceneNavigationUIManager : Singleton<MenuSceneNavigationUIManager> {
	public GameObject menuSceneNavigationPanel;

	public void ShowPanel(){
		menuSceneNavigationPanel.GetComponent<TweenToggleDemux>().Show();
	}
	
	public void HidePanel(){
		menuSceneNavigationPanel.GetComponent<TweenToggleDemux>().Hide();
	}
}
