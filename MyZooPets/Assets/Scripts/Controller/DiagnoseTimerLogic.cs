using UnityEngine;
using System.Collections;

//this logic should be in any class that an asthma flare up could happen
//when the flare up happens this logic will notify the NotificationUIManager
//and pauses the game
public class DiagnoseTimerLogic : MonoBehaviour {
    private float timer = 0;
    private float timeInterval = 30f; //time interval for triggers to affect health
    private bool turnOffDiagnoseTimer; //For Testing

	// Use this for initialization
	void Start () {
        timer = timeInterval;
        turnOffDiagnoseTimer = true;
	}

	// Update is called once per frame
	void Update () {
        if(turnOffDiagnoseTimer) return;

        if (!LoadLevelManager.IsPaused){
            timer -= Time.deltaTime;
            if (timer <= 0){
                timer = timeInterval;
                SendNotification();
                turnOffDiagnoseTimer = true;
            }
        }
	}

    public void Init(){
        timer = timeInterval;
        turnOffDiagnoseTimer = false;
    }

    private void SendNotification(){
        NotificationUIManager.Instance.PopupNotificationTwoButtons("Something unusual is happening to you pet! Help it out!",
            delegate(){
                Application.LoadLevel("DiagnosePet");
            },
            delegate(){
                //ignore. no punishment. unpause the game
                //fewer rewards
            });
    }
}
