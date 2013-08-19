using UnityEngine;
using System.Collections;

//this logic should be in any class that an asthma flare up could happen
//when the flare up happens this logic will notify the NotificationUIManager
//and pauses the game
public class DiagnoseTimerLogic : MonoBehaviour {
    private float timer = 0;
    private float timeInterval = 30f; //time interval for triggers to affect health

	// Use this for initialization
	void Start () {
        timer = timeInterval;
	}

	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0){
            timer = timeInterval;
            SendNotification();
        }
	}

    //Increases the chance of this happening if health is low
    private void SendNotification(){
        NotificationUIManager.Instance.EnqueuePopupNotificationTwoButtons("Something unusual is happening to your pet! Help it out!",
            delegate(){
                Application.LoadLevel("DiagnosePet");
            },
            delegate(){
                //ignore. no punishment. unpause the game
                //fewer rewards
            });
    }
}
