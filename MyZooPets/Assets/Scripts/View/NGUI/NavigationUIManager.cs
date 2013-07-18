using UnityEngine;
using System.Collections;

public class NavigationUIManager : MonoBehaviour {

    public ClickManager clickManager;
    public UILabel yardLabel;

    private bool isInRoom;

	// Use this for initialization
	void Start () {
	   if(Application.loadedLevelName =="NewBedRoom"){
            yardLabel.text = "Yard";
            isInRoom = true;
       }else if(Application.loadedLevelName == "Yard"){
            yardLabel.text = "Room";
            isInRoom = false;
       }else{}

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
