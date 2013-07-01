using UnityEngine;
using System.Collections;

public class LevelUpGUI : MonoBehaviour {
    public NotificationUIManager notificationUIManager;
    private LevelUpLogic levelUpLogic; //reference 

	// Use this for initialization
	void Start () {
	   levelUpLogic = GameObject.Find("GameManager").GetComponent<LevelUpLogic>();
       levelUpLogic.OnLevelUp = OnLevelUpNotification;
	}

    private void OnLevelUpNotification(TrophyTier trophy){
        notificationUIManager.PopupImageMessage(trophy, null);
    }
}
