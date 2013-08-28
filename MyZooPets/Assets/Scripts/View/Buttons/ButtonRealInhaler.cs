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
			NotificationUIManager.Instance.EnqueuePopupNotificationOneButton(
				"I don't need this right now.",
				delegate(){}
			);
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
