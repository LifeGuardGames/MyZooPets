using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour {
    private const int BUTTON_OFFSET = 55;
    private const int BUTTON_HEIGHT = 50;
    private const int BUTTON_WIDTH = 100;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    void OnGUI(){
        //1 button popup notification
        if(GUI.Button(new Rect(10, 10, BUTTON_WIDTH, BUTTON_HEIGHT), "one button")){
            NotificationUIManager.Instance.PopupNotificationOneButton("testing", null);
        }

        //level up notification
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*1, BUTTON_WIDTH, BUTTON_HEIGHT),
            "level up")){
            NotificationUIManager.Instance.LevelUpMessage(BadgeTier.Bronze, null);
        }

        //popup texture "great"
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*2, BUTTON_WIDTH, BUTTON_HEIGHT),
            "great")){
            NotificationUIManager.Instance.PopupTexture("great");
        }

        //game over reward
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*3, BUTTON_WIDTH, BUTTON_HEIGHT),
            "GG Reward")){
            NotificationUIManager.Instance.GameOverRewardMessage(1000, 0, null, null);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*4, BUTTON_WIDTH, BUTTON_HEIGHT),
            "two button")){
            NotificationUIManager.Instance.PopupNotificationTwoButtons("testing", null, null);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*5, BUTTON_WIDTH, BUTTON_HEIGHT),
            "1 button GG")){
            NotificationUIManager.Instance.GameOverRewardMessage(1000, 10000, null);
        }

        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*6, BUTTON_WIDTH, BUTTON_HEIGHT),
            "tutorial test")){
            NotificationUIManager.Instance.TutorialMessage(TutorialImageType.CalendarGreenStamp,
               null, "", "");
        }
    }
}
