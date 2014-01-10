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

	private int nativeWidth;
	public int GetNativeWidth() {
		return nativeWidth;	
	}
	
	private int nativeHeight;
	public int GetNativeHeight() {
		return nativeHeight;	
	}
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Awake(){
		// native height is a fixed constant that we define for NGUI
		nativeHeight = Constants.GetConstant<int>("NativeHeight");
		ratioY = nativeHeight/(Screen.height * 1.0f);
		
		// native width is not a constant -- it is a thing created by NGUI based on the height
		nativeWidth = (int) ( Screen.width * ratioY );
		ratioX = nativeWidth/(Screen.width * 1.0f);
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
		PanToMoveCamera script = GetPanScript();
		
		// ahhhhhhhhh....hack for screens that do not have panning....
		if ( script == null )
			return false;
		
		GameObject goParent = transform.parent.gameObject;
		
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
	// Transform world position to screen position.
	// Only use this to transform world position for use with NGUI! 
	//---------------------------------------------------		
	public Vector3 WorldToScreen( Camera cam, Vector3 vPos ){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = cam.WorldToScreenPoint(vPos);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}

	public Vector3 ViewportToScreen( Camera cam, Vector3 vPos ){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = cam.ViewportToScreenPoint(vPos);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}

	private Vector3 ScalePositionForNGUI(Vector3 vPos){
		Vector3 scaledRatioScreenPos = new Vector3(vPos.x * ratioX, vPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}
	
	//---------------------------------------------------
	// TransformAnchorPosition()
	// Strictly for use with NGUI, takes a vector and the
	// anchor that vector is in (eAnchorIn) and transforms
	// that position to coordinates as if the position were
	// in the eAnchorOut anchor.
	//---------------------------------------------------	
	public Vector3 TransformAnchorPosition( Vector3 vPos, InterfaceAnchors eAnchorIn, InterfaceAnchors eAnchorOut ) {
		// no need to do any transforming if the two anchors are the same
		if ( eAnchorIn == eAnchorOut )
			return vPos;
		
		Vector3 vTransformed = vPos;
		
		switch ( eAnchorOut ) {
			case InterfaceAnchors.Center:
				vTransformed = TransformAnchor_Center( vPos, eAnchorIn );
				break;
			case InterfaceAnchors.Top:
				vTransformed = TransformAnchor_Top( vPos, eAnchorIn );
				break;
			default:
				Debug.Log("Sorry, anchor not supported yet");
				break;
		}
		
		return vTransformed;
	}
	
	private Vector3 TransformAnchor_Center( Vector3 vPos, InterfaceAnchors eAnchorIn ) {
		Vector3 vTransformed = vPos;
		
		switch ( eAnchorIn ) {
			case InterfaceAnchors.BottomLeft:
				vTransformed.x -= nativeWidth / 2;
				vTransformed.y -= nativeHeight / 2;
				break;
			case InterfaceAnchors.Bottom:
				vTransformed.y -= nativeHeight / 2;
				break;
			case InterfaceAnchors.Top:
				vTransformed.x -= nativeWidth / 2;
				vTransformed.y += nativeHeight / 2;
				break;
			default:
				Debug.Log("Sorry future team, Joe did not implement the feature you're looking for yet.");
				break;
		}
		
		return vTransformed;		
	}
	
	private Vector3 TransformAnchor_Top( Vector3 vPos, InterfaceAnchors eAnchorIn ) {
		Vector3 vTransformed = vPos;
		
		switch ( eAnchorIn ) {
			case InterfaceAnchors.Center:
				// vTransformed.x += nativeWidth / 2; //Jason: This doesn't make sense for center to top conversion
													 // so i commented it out. center to top has the same x only the
													 //Y change...not sure why this was here in the beginning. hope i
													 //didn't break more code
				vTransformed.y -= nativeHeight / 2;
				break;
			case InterfaceAnchors.BottomLeft:
				vTransformed.x -= nativeWidth / 2;
				vTransformed.y -= nativeHeight;
				break;			
			default:
				Debug.Log("Sorry future team, Joe did not implement the feature you're looking for yet.");
				break;
		}
		
		return vTransformed;		
	}	
}
