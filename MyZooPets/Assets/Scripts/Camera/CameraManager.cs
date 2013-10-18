using UnityEngine;
using System.Collections;

//---------------------------------------------------
// CameraManager
// Right now, this script really just manages the
// perspective camera.
//---------------------------------------------------

public class CameraManager : Singleton<CameraManager> {
	// default positoin/rotation of the camera
	protected Vector3 initPosition;	// Default position: 0, 5.7, -23
	protected Vector3 initFaceDirection;	
	
	// is the camera zoomed?
	private bool bZoomed;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	protected virtual void Start(){
	}	

	//---------------------------------------------------
	// ZoomToTarget()
	// Moves the camera to a target position with a 
	// target rotation over a set time.
	//---------------------------------------------------	
	public void ZoomToTarget( Vector3 vPos, Vector3 vRotation, float fTime, GameObject goObject ) {
		// before zooming, cache the camera position
        initPosition = gameObject.transform.position;
        initFaceDirection = gameObject.transform.eulerAngles;
		
		// move the camera
		MoveCamera( vPos, vRotation, fTime, goObject );
			
		// mark that the camera is zoomed
		bZoomed = true;
	}
	
	//---------------------------------------------------
	// ZoomOutMove()
	// Zooms the camera back to its original position
	// over time.
	//---------------------------------------------------	
	public void ZoomOutMove(){
		ZoomOutMove(.3f);
	}
	public void ZoomOutMove(float time){
		if (bZoomed){
			MoveCamera(initPosition,initFaceDirection, time, null);
			bZoomed = false;
		}
	}	
	
	//---------------------------------------------------
	// MoveCamera()
	//---------------------------------------------------	
	private void MoveCamera( Vector3 vPos, Vector3 vRotation, float fTime, GameObject goObject ) {
		// set up the movement hash
		Hashtable hashMove = new Hashtable();
		
		// make sure to subtract the camera's parent's position from the vPos because the parent moves as the partitions pan
		vPos -= gameObject.transform.parent.position;
		
		// if the incoming object isn't null, set up a callback
		if ( goObject != null ) {
			hashMove.Add("onCompleteTarget", goObject);
			hashMove.Add("onComplete", "CameraMoveDone");
		}
		
		hashMove.Add("ease", LeanTweenType.easeInOutQuad);
		
		// set up the roation hash
		Hashtable hashRotation = new Hashtable();
		hashRotation.Add("ease", LeanTweenType.easeInOutQuad);
		
		// kick of the move and rotation tweens
		LeanTween.moveLocal(gameObject, vPos, fTime, hashMove);
		LeanTween.rotateLocal(gameObject, vRotation, fTime, hashRotation);		
	}
}
