
using UnityEngine;
using System.Collections;

/// <summary>
/// Camera follow. Camera follows the player
/// </summary>
public class CameraFollow : MonoBehaviour {
    public MegaHazard MegaHazardToOffset;
	public Transform playerTransform;

	private Vector3 cameraPositionOffset;
	public float minOffset = 5; //Used to be main camera's transform, now moved to here (13.40473f)
	public float maxOffset = 18;
	private float currentOffset = 0;
	private float maxSpeed;
	private float minSpeed;
	private float speedPercentage;
	//private float playerOffset = ; 
	// Use this for initialization
	void Start () {
		//just hard set this value so that the player is always at the end of the camera
//		cameraPositionOffset = new Vector3(30, 0, 0); //this value takes acount of the mega hazard offset
		maxSpeed = PlayerController.Instance.GetMaxSpeed();
		minSpeed = PlayerController.Instance.GetMinSpeed();
		speedPercentage = -minSpeed / (maxSpeed-minSpeed); //Starts at speed = 0 so adjust camera accordinly
		currentOffset = maxOffset + (minOffset - maxOffset) * speedPercentage;
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
		speedPercentage = (PlayerController.Instance.GetCurrentSpeed()-minSpeed)/(maxSpeed-minSpeed); //Between 0 and 1 depending on percentage of currentSpeed from minSpeed to maxSpeed
		currentOffset = maxOffset + (minOffset - maxOffset) * speedPercentage; //Now make our offset somewhere between maxOffset and minOffset depending on speedPercentage
		this.transform.position = new Vector3(playerTransform.position.x+currentOffset, 14, this.transform.position.z); //playerTransform.position.x+offset
	}
}