using UnityEngine;
using System.Collections;

public class BallBouncing : MonoBehaviour {

	public Vector3 cameraZoom;
	public int combo;
	public GameObject zoomItem;
	public float force;
	public GameObject backButton;
	public GameObject comboLabel;
	public bool isActive;

	// Use this for initialization
	void Start () {
	
	}
	/*void openBallGame(){
		if(!isActive){
			// Zoom into the item
			Vector3 targetPosition = zoomItem.transform.position + cameraZoom;
			
			CameraManager.Callback cameraDoneFunction = delegate(){
				CameraMoveDone();
			};
			CameraManager.Instance.ZoomToTarget(targetPosition, zoomRotation, ZoomTime, cameraDoneFunction);
			
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			
			//need to disable more things here
			PetAnimationManager.Instance.DisableIdleAnimation();
			isActive = true;
			backButton.SetActive(true);
		}
	
	}*/


	// Update is called once per frame
	void Update () {
	
	}
}
