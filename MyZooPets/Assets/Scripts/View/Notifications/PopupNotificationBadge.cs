using UnityEngine;
using System.Collections;

public class PopupNotificationBadge : PopupNotificationWithImageNGUI {
	
	public UILabel badgeTitle;
	public UILabel badgeDescription;
	
	public void setTitle(string title){
		badgeTitle.text = title; 
	}
	
	public void setDescription(string desc){
		badgeDescription.text = desc;
	}
}
