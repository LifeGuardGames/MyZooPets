using UnityEngine;
using System.Collections;

public class FlameLevelUpNotificationListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
        FlameLevelLogic.OnFlameLevelUp += OnFlameLevelUp;	
	}

    void OnDestroy(){
        FlameLevelLogic.OnFlameLevelUp -= OnFlameLevelUp;   
    }

    private void OnFlameLevelUp(object sender, FlameLevelLogic.FlameLevelEventArgs args){
        //Unregister handler so we don't get multiple same notifications
        FlameLevelLogic.OnFlameLevelUp -= OnFlameLevelUp;

        PopupNotificationNGUI.Callback button1Function = delegate(){
            //unregister before registering listener to prevent multiple registering
            //when using spam click the button
            FlameLevelLogic.OnFlameLevelUp -= OnFlameLevelUp;
            FlameLevelLogic.OnFlameLevelUp += OnFlameLevelUp;
        };

        Skill skill = args.UnlockedSkill;    
        Hashtable notificationEntry = new Hashtable();

        //notificationEntry.Add(NotificationPopupData.Type, NotificationPopupType.FireLevelUp);
        //notificationEntry.Add(NotificationPopupData.Message, skill.Description); 
        //notificationEntry.Add(NotificationPopupData.SpriteName, skill.TextureName);
        //notificationEntry.Add(NotificationPopupData.Button1Callback, button1Function);
        
        NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
    }
	
}
