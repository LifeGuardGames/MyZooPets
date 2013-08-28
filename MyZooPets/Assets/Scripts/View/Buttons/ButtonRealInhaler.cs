using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonRealInahler
// Button class that loads up the real inhaler game.
//---------------------------------------------------

public class ButtonRealInhaler : LgButton {
	
	public CameraMove cameraMove;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		if (DataManager.Instance.Tutorial.FirstTimeRealInhaler){
			return; // taken care of in Tutorial.cs
		}
		if (CalendarLogic.CanUseRealInhaler){
			OpenRealInhaler();
		}
		else {
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){};
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.OneButton);
			notificationEntry.Add(NotificationPopupFields.Message, "I don't need this right now.");
			notificationEntry.Add(NotificationPopupFields.Button1Label, "Back");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);			
		}
	}
	
	//---------------------------------------------------
	// OpenRealInhaler()
	// Also called from tutorial as a callback.
	//---------------------------------------------------
	public void OpenRealInhaler(){
		cameraMove.ZoomToggle(ZoomItem.RealInhaler);
		ClickManager.Instance.ClickLock();
		ClickManager.Instance.ModeLock();

		//Hide other UI Objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
	}	
}
