using UnityEngine;
using System.Collections;

public class NavigationUIManager : MonoBehaviour {

	public GameObject navigationPanel;
    public ClickManager clickManager;

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

	public void ShowPanel(){
		navigationPanel.GetComponent<MoveTweenToggleDemultiplexer>().Show();
	}

	public void HidePanel(){
		navigationPanel.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
	}
}
