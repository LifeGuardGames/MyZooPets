using UnityEngine;
using System.Collections;

/// <summary>
/// This script makes its gameobject follow another item via raycasting on from screen
/// Ideally this would be used in between two objects from two different cameras
/// </summary>
public class FollowObjectRaycast : MonoBehaviour {

	public GameObject target;
	private Vector3 targetAuxPosition;

	void Start(){
		if(target != null){
			RaycastAndMove();
		}
	}

	void Update(){
		if(target != null){
			// Only do raycast of the position of the target has changed since the last frame
			if(target.transform.position != targetAuxPosition){
				RaycastAndMove();
			}
		}
	}

	private void RaycastAndMove(){
		// Raycast and move
		gameObject.transform.localPosition = CameraManager.Instance.WorldToScreen(Camera.main, target.transform.position);

		// Keep track to check if moved later on
		targetAuxPosition = target.transform.position;
	}
}
