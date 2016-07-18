using UnityEngine;
using System.Collections;

/// <summary>
/// Camera follow. Camera follows the player
/// </summary>
public class CameraFollow : MonoBehaviour {
    public MegaHazard MegaHazardToOffset;
	public Transform playerTransform;

	private Vector3 cameraPositionOffset;
	
	// Use this for initialization
	void Start () {

		//just hard set this value so that the player is always at the end of the camera
//		cameraPositionOffset = new Vector3(30, 0, 0); //this value takes acount of the mega hazard offset
	}

	void Update(){
//		Vector3 oldPosition = transform.position;
//
//		// Whenever we move, move the camera relative to user
//		Vector3 newPosition = playerTransform.position + cameraPositionOffset;
//		
//		//Make sure to consider the mega hazard offset ass well
//		float hazardOffset = MegaHazardToOffset.GetCurrentOffsetDistance();
//		newPosition.x += hazardOffset;
//
//
//		//Camera only follows the players' x position
//		newPosition = new Vector3(newPosition.x, 16, -12);
//
////		FollowingCamera.transform.position = Vector3.Lerp(oldPosition, newPosition, Time.deltaTime * 2f);
//		transform.position = newPosition;


		this.transform.position = new Vector3(this.transform.position.x, 14, this.transform.position.z);
	}
}