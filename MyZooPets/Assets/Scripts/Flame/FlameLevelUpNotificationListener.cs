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

        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.FireLevelUp);
        notificationEntry.Add(NotificationPopupFields.Message, skill.Description); 
        notificationEntry.Add(NotificationPopupFields.SpriteName, skill.TextureName);
        notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
        
        NotificationUIManager.Instance.AddToQueue(notificationEntry);

        //Sen Analytics event
        Analytics.Instance.FlameUnlocked(skill.ID);
    }
	
}
