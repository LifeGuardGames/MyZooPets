using UnityEngine;
using System.Collections;

public class NavigationUIManager : MonoBehaviour {

    public ClickManager clickManager;
    private bool isInRoom;

	void Start () {
	   if(Application.loadedLevelName =="NewBedRoom"){
            isInRoom = true;
       }else if(Application.loadedLevelName == "Yard"){
            isInRoom = false;
       }
	}

    public void NavigationButtonClicked(GameObject button){
       switch(button.name){
            case "Note":
                clickManager.OnClickNote();
            break;
            case "Store":
                clickManager.OnClickStore();
            break;
            case "Yard":
                if(isInRoom){
                    Application.LoadLevel("Yard");
                }else{
                    Application.LoadLevel("NewBedRoom");
                }
            break;
            default:
            break;
       }
    }
}
