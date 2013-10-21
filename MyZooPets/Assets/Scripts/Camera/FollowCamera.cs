/* 
 * Description:
 * Drags the attached camera along this objects transform.
 * Also, if a megahazard exists, it responds to the given offset.
 * This gives the illusion of the player moving backwards while running.
 */

using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	public Camera FollowingCamera;
    public MegaHazard MegaHazardToOffset;
	
	private Vector3 mCameraRelativeVector;
	
	// Use this for initialization
	void Start () {
		mCameraRelativeVector = FollowingCamera.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Whenever we move, move the camera relative to user
		Vector3 newPosition = transform.position + mCameraRelativeVector;

        //Make sure to consider the mega hazard offset ass well
        float hazardOffset = MegaHazardToOffset.GetCurrentOffsetDistance();
        newPosition.x += hazardOffset;

        //Camera only follows the players' x position
        Vector3 cameraPos = FollowingCamera.transform.position;
		FollowingCamera.transform.position = new Vector3(newPosition.x, cameraPos.y, cameraPos.z);
	}
}