using UnityEngine;
using System.Collections;

public class AccessoryUIManager : SingletonUI<AccessoryUIManager> {
	public GameObject backButton;
	public GameObject zoomItem;
	private bool isActive = false;

	// related to zooming into the badge board
	public float fZoomTime;
	public Vector3 vOffset;
	public Vector3 vRotation;

	//When the badge board is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			// zoom into the board
			Vector3 vPos = zoomItem.transform.position + vOffset;
			CameraManager.Instance.ZoomToTarget( vPos, vRotation, fZoomTime, null );
			
			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			RoomArrowsUIManager.Instance.HidePanel();
			
			isActive = true;
			zoomItem.collider.enabled = false;
			
			backButton.SetActive(true);
		}
	}
	
	//The back button on the left top corner is clicked to zoom out of the badge board
	protected override void _CloseUI(){
		if(isActive){
			isActive = false;
			zoomItem.collider.enabled = true;
			
			CameraManager.Instance.ZoomOutMove();
			
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();
			RoomArrowsUIManager.Instance.ShowPanel();

			backButton.SetActive(false);
		}
	}
}
