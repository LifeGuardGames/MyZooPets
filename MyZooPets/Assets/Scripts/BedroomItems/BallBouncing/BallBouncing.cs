using UnityEngine;
using System.Collections;

public class BallBouncing : MonoBehaviour {

	public Vector3 cameraZoom;
	public int combo;
	public GameObject zoomItem;
	public Vector3 zoomRotation;
	public float zoomTime;
	public GameObject backButton;
	public GameObject comboLabel;
	public bool isActive = false;
	public Vector2 force;

	// Use this for initialization
	void Start () {
	
	}
	void openBallGame(){
		/*if(!isActive){
			// Zoom into the item
			Vector3 targetPosition = zoomItem.transform.position + cameraZoom;
			
			CameraManager.Callback cameraDoneFunction = delegate(){
			};
			CameraManager.Instance.ZoomToTarget(targetPosition, zoomRotation, zoomTime, cameraDoneFunction);
			MovePet();
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			
			//need to disable more things here
			PetAnimationManager.Instance.DisableIdleAnimation();
			isActive = true;
			backButton.SetActive(true);
			PetMovement.Instance.canMove = false;
		}*/
	
	}

	/// <summary>
	/// Move pet into ball view after camera is done zooming in
	/// </summary>
	private void MovePet(){
		GameObject pet = GameObject.Find("Pet");
		//teleport first then walk into view
		if(!pet.renderer.isVisible)
			PetMovement.Instance.petSprite.transform.position = new Vector3(transform.position.x-4f, 0, 26.65529f);
		PetMovement.Instance.MovePet(this.gameObject.transform.position);
		
	}

	void OnTap (TapGesture e){
		if(!isActive){
			this.rigidbody.useGravity = true;
		}
		else{
		combo++;
		transform.rigidbody2D.AddForceAtPosition(force, e.Position);
		}
	}

}
