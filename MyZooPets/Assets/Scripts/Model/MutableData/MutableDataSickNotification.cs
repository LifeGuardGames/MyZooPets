using UnityEngine;
using System.Collections;

public class MutableDataSickNotification{

	public bool IsRemindedThisPlayPeriod {get; set;}
	public int NumOfRemindersLeft {get; set;}
	
	public bool IsReminderActive{
		get{
			return NumOfRemindersLeft > 0;
		}
	}

	public void DecreaseReminderCount(){
		NumOfRemindersLeft--;
	}

	public MutableDataSickNotification(){
		IsRemindedThisPlayPeriod = false;
		NumOfRemindersLeft = 3;
	}
}
