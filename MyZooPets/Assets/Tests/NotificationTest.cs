using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour {
    public NotificationUIManager notificationUIManager;
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
            notificationUIManager.PopupNotification("testing", null);
        }

        //level up notification
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*1, BUTTON_WIDTH, BUTTON_HEIGHT),
            "level up")){
            notificationUIManager.PopupImageMessage(TrophyTier.Bronze, null);
        }

        //popup texture "great"
        if(GUI.Button(new Rect(10, 10+BUTTON_OFFSET*2, BUTTON_WIDTH, BUTTON_HEIGHT),
            "great")){
            notificationUIManager.PopupTexture("great");
        }
    }
}
