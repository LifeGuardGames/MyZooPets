using UnityEngine;
using System.Collections;

//---------------------------------------------------
// CameraManager
// Right now, this script really just manages the
// perspective camera.
//---------------------------------------------------

public class CameraManager : Singleton<CameraManager> {
	// cameras
	public Camera cameraMain;
	public Camera cameraNGUI;
	
	// ratios
	public float ratioX;
	public float ratioY;
	
	// default positoin/rotation of the camera
	protected Vector3 initPosition;	// Default position: 0, 5.7, -23
	protected Vector3 initFaceDirection;	
	
	// pan to move script
	public PanToMoveCamera scriptPan;
	public PanToMoveCamera GetPanScript() {
		return scriptPan;	
	}
	
	// is the camera zoomed?
	private bool bZoomed;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Awake(){
		ratioX = 1280f/(Screen.width * 1.0f);
		ratioY = 800f/(Screen.height * 1.0f);		
	}	
	
	//---------------------------------------------------
	// IsPartitionChanging()
	// Returns true if the camera is currently moving.
	// NOTE: I used to check for a lean tween, but we
	// sometimes move the camera without using lean tween.
	// So now I just look at the X of the camera and see if
	// is where it should be.
	//---------------------------------------------------		
	public bool IsCameraMoving() {
		GameObject goParent = transform.parent.gameObject;
		
		PanToMoveCamera script = GetPanScript();
		float fTargetX = script.partitionOffset * script.currentPartition;
		float fX = goParent.transform.position.x;
		
		bool bMoving = fTargetX != fX;
		
		return bMoving;
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
	
	//---------------------------------------------------
	// WorldToScreen()
	//---------------------------------------------------		
	public Vector3 WorldToScreen( Camera cam, Vector3 vPos ){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = cam.WorldToScreenPoint(vPos);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}
	
	//---------------------------------------------------
	// BottomLeftToCenter()
	// This function is specifically for tranforming NGUI
	// coordinates from the bottom left anchor to the 
	// center anchor.  This is because the bottom left
	// anchor matches screen position (bottom left = 0,0),
	// whereas the center anchor is 0,0 in the center.
	// We may need to change this a little bit more to
	// accomodate Android devices.
	//---------------------------------------------------	
	public Vector3 BottomLeftToCenter( Vector3 vPos ) {
		Vector3 vCenter = vPos;
		vCenter.x -= Screen.width / 2;
		vCenter.y -= Screen.height / 2;
		
		return vCenter;
	}
}
