using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	public Camera FollowingCamera;
    public MegaHazard MegaHazardToOffset;
	
	private Vector3 mCameraRelativeVector;
	
	// Use this for initialization
	void Start () {
		mCameraRelativeVector = FollowingCamera.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Whenever we move, move the camera relative to use.
		Vector3 newPosition = transform.position + mCameraRelativeVector;

        // o wait, is there a megahazard to pull an offset from?
        if (MegaHazardToOffset != null) {
            float hazardOffset = MegaHazardToOffset.GetCurrentOffsetDistance();
            newPosition.z += hazardOffset;
        }

		FollowingCamera.transform.position = newPosition;
	}
}