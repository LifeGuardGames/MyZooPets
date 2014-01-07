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
        Skill skill = args.UnlockedSkill;    
        Hashtable notificationEntry = new Hashtable();

        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.FireLevelUp);
        notificationEntry.Add(NotificationPopupFields.Message, skill.Description); 
        notificationEntry.Add(NotificationPopupFields.SpriteName, skill.TextureName);
        notificationEntry.Add(NotificationPopupFields.Button1Callback, null);
        
        NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }
	
}
