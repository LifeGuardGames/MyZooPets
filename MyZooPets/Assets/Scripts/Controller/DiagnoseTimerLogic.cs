using UnityEngine;
using System.Collections;

//this logic should be in any class that an asthma flare up could happen
//when the flare up happens this logic will notify the NotificationUIManager
//and pauses the game
public class DiagnoseTimerLogic : MonoBehaviour {
    public NotificationUIManager notificationUIManager;

	// Use this for initialization
	void Start () {
	   notificationUIManager = GameObject.Find("Main Camera/NotificationUIManager").GetComponent<NotificationUIManager>();
        // SendNotification();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SendNotification(){
        notificationUIManager.PopupNotification("Your pet is not feeling well! It needs your help!",
            delegate(){
                Application.LoadLevel("DiagnosePet");
            },
            delegate(){
                //ignore. no punishment. unpause the game
                //fewer rewards
            });
    }
}
