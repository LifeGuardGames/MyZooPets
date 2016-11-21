using UnityEngine;
using System.Collections;

//---------------------------------------------------
// FlameChartUIManager
// UI Manager for the fire chart that displays the
// user's current and future flames.
//---------------------------------------------------

public class FlameChartUIManager : SingletonUI<FlameChartUIManager> {
	// zoom helper
//	public ZoomHelper zoomHelper;
	
	// back button for the chart
	public GameObject goBackButton;
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------
	protected override void _OpenUI(){		
		// zoom into the chart
//		zoomHelper.Zoom();
		
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		
		// disable the collider so the user can't click the chart again
		gameObject.collider.enabled = false;

		// enable the back button for the user to back out
		goBackButton.SetActive(true);
	}

	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------
	protected override void _CloseUI(){
		// enable the collider so that the board can be clicked again
		gameObject.collider.enabled = true;
		
		// zoom out
		CameraManager.Instance.ZoomOutMove();

		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();

		// deactivate the back button
		goBackButton.SetActive(false);
	}	
}
