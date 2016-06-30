using UnityEngine;

/// <summary>
/// Camera follow. Camera follows the player
/// </summary>
public class CameraFollow : MonoBehaviour {
	public Transform playerTransform;
	private Vector3 cameraPositionOffset;
	public float minOffset = 5;
	//Used to be main camera's transform, now moved to here (13.40473f)
	public float maxOffset = 18;
	public float height = 17;
	private float currentOffset = 0;
	private float maxSpeed;
	private float minSpeed;
	private float speedPercentage;
	bool following = false;
	//private float playerOffset = ;
	// Use this for initialization
	void Start() {
		maxSpeed = PlayerController.Instance.MaxSpeed;
		minSpeed = PlayerController.Instance.MinSpeed;

	}

	void Update() {
		if (!following) {
			if (playerTransform.position.x > (transform.position.x - 20)) { //Let the player get a little ahead
				following = true;
				speedPercentage = -minSpeed / (maxSpeed - minSpeed); //Starts at speed = 0 so adjust camera accordinly
				currentOffset = maxOffset + (minOffset - maxOffset) * speedPercentage;
				ParallaxingBackgroundManager.Instance.PlayParallax();
			}
			return;
		}
		//We are now following the player, so calculate our offset based on the player's current speed and its total and put us at the offset
		speedPercentage = (PlayerController.Instance.Speed - minSpeed) / (maxSpeed - minSpeed); //Between 0 and 1 depending on percentage of currentSpeed from minSpeed to maxSpeed
		currentOffset = maxOffset + (minOffset - maxOffset) * speedPercentage; //Now make our offset somewhere between maxOffset and minOffset depending on speedPercentage
		transform.position = new Vector3(playerTransform.position.x + currentOffset, height, this.transform.position.z); //playerTransform.position.x+offset
	}

	public void Reset() {
		following = false;
		transform.position = playerTransform.position + new Vector3(42f, 15f, -14f);
	}
}