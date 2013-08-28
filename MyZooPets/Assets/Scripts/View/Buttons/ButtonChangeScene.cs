using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonChangeScene
// Generic button class that zooms in on an item and
// then changes the scene.  Note that the zoom 
// function in CameraMove does the actual loading.
//---------------------------------------------------

public class ButtonChangeScene : LgButton {
	
	public CameraMove cameraMove;
	public ZoomItem zoomItem;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		cameraMove.ZoomToggle(zoomItem);
		ClickManager.Instance.ClickLock();
		ClickManager.Instance.ModeLock();
	}	
}
