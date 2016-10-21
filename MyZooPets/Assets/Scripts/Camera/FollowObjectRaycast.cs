using UnityEngine;

/// <summary>
/// This script makes its gameobject follow another item via raycasting on from screen
/// Ideally this would be used in between two objects from two different cameras
/// </summary>
public class FollowObjectRaycast : MonoBehaviour {

	public GameObject target;
	public Camera mainCamera;

	//private Vector3 targetAuxPosition;
	//private Vector3 mainCameraAuxPosition;

	void Start(){
		if(mainCamera == null){
			mainCamera = Camera.main;
		}
		gameObject.transform.localPosition = target.transform.localPosition;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x +200, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
		//RaycastAndMove();
	}

//	void Update(){
		// Only do raycast of the position of the target has changed since the last frame, account for camera move too
	//	if(target.transform.position != targetAuxPosition || mainCamera.transform.position != mainCameraAuxPosition){
	//		RaycastAndMove();
	//	}
//	}

	private void RaycastAndMove(){
		// Raycast and move
		gameObject.transform.localPosition = CameraManager.Instance.WorldToScreen(Camera.main, target.transform.position);

		// Keep track to check if moved later on
		//targetAuxPosition = target.transform.position;
		//mainCameraAuxPosition = mainCamera.transform.position;
	}
}
