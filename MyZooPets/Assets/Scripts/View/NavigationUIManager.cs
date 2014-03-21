using UnityEngine;
using System.Collections;

public class NavigationUIManager : Singleton<NavigationUIManager> {
	public GameObject navigationPanel;
	
	// used with tutorials
	public GameObject goEditDecos;
	public GameObject GetEditDecoButton() {
		return goEditDecos;	
	}
	
	/*
    public void NavigationButtonClicked(GameObject button){
       switch(button.name){
            case "Note":
                clickManager.OnClickNote();
            break;
            case "Store":
                clickManager.OnClickStore();
            break;
            default:
            break;
       }
    }
    */

	public void ShowPanel(){
		navigationPanel.GetComponent<TweenToggleDemux>().Show();
	}

	public void HidePanel(){
		navigationPanel.GetComponent<TweenToggleDemux>().Hide();
	}
}
