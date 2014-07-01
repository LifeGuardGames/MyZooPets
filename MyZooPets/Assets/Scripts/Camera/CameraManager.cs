using UnityEngine;
using System.Collections;

//---------------------------------------------------
// CameraManager
// Right now, this script really just manages the
// perspective camera.
//---------------------------------------------------

public class CameraManager : Singleton<CameraManager>{
	// cameras
	public Camera cameraMain;
	public Camera cameraNGUI;
	
	// ratios
	public float ratioX;
	public float ratioY;

	public float GetRatioDifference(){
		return ratioY;
	}
	
	// default positoin/rotation of the camera
	protected Vector3 initPosition;	// Default position: 0, 5.7, -23
	protected Vector3 initFaceDirection;	
	
	// pan to move script
	public PanToMoveCamera scriptPan;

	public PanToMoveCamera GetPanScript(){
		return scriptPan;	
	}
	
	// is the camera zoomed?
	private bool bZoomed;
	private bool bZooming;

	public void SetZooming(bool b){
		bZooming = b;	
	}

	private int nativeWidth;

	public int GetNativeWidth(){
		return nativeWidth;	
	}
	
	private int nativeHeight;

	public int GetNativeHeight(){
		return nativeHeight;	
	}
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Awake(){
		// native height is a fixed constant that we define for NGUI
		nativeHeight = Constants.GetConstant<int>("NativeHeight");
		ratioY = nativeHeight / (Screen.height * 1.0f);
		
		// native width is not a constant -- it is a thing created by NGUI based on the height
		nativeWidth = (int)(Screen.width * ratioY);
		ratioX = nativeWidth / (Screen.width * 1.0f);
	}	
	
	//---------------------------------------------------
	// IsPartitionChanging()
	// Returns true if the camera is currently moving.
	// NOTE: I used to check for a lean tween, but we
	// sometimes move the camera without using lean tween.
	// So now I just look at the X of the camera and see if
	// is where it should be.
	//---------------------------------------------------		
	public bool IsCameraMoving(){
		PanToMoveCamera script = GetPanScript();
		
		// ahhhhhhhhh....hack for screens that do not have panning....
		if(script == null)
			return false;
		
		GameObject goParent = transform.parent.gameObject;
		
		float fTargetX = script.partitionOffset * script.currentPartition;
		float fX = goParent.transform.position.x;
		
		// check if the camera is moving horizontally or zooming
		bool bMoving = fTargetX != fX || bZooming;
		
		return bMoving;
	}
	
	//---------------------------------------------------
	// ZoomToTarget()
	// Moves the camera to a target position with a 
	// target rotation over a set time.
	// NOTE: If this function can ever "fail", be sure to
	// check ZoomHelper because it assumes this function
	// will always work.
	//---------------------------------------------------	
	public void ZoomToTarget(Vector3 vPos, Vector3 vRotation, float fTime, GameObject goObject){
		// before zooming, cache the camera position
		initPosition = gameObject.transform.position;
		initFaceDirection = gameObject.transform.eulerAngles;
		
		// move the camera
		MoveCamera(vPos, vRotation, fTime, goObject);
			
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
		if(bZoomed){
			MoveCamera(initPosition, initFaceDirection, time, null);
			bZoomed = false;
		}
	}	
	
	//---------------------------------------------------
	// MoveCamera()
	//---------------------------------------------------	
	private void MoveCamera(Vector3 vPos, Vector3 vRotation, float fTime, GameObject goObject){
		// set up the movement hash
		Hashtable hashMove = new Hashtable();
		
		// make sure to subtract the camera's parent's position from the vPos because the parent moves as the partitions pan
		vPos -= gameObject.transform.parent.position;
		
		// if the incoming object isn't null, set up a callback
		if(goObject != null){
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

	/// <summary>
	/// Worlds to screen.
	/// Transform world position to screen position.
	/// Only use this to transform world position for use with NGUI! 
	/// </summary>
	/// <returns>screen position.</returns>
	/// <param name="cam">Cam.</param>
	/// <param name="vPos">position.</param>
	public Vector3 WorldToScreen(Camera cam, Vector3 position){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = cam.WorldToScreenPoint(position);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}

	public Vector3 ViewportToScreen(Camera cam, Vector3 vPos){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = cam.ViewportToScreenPoint(vPos);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}

	private Vector3 ScalePositionForNGUI(Vector3 vPos){
		Vector3 scaledRatioScreenPos = new Vector3(vPos.x * ratioX, vPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}

	/// <summary>
	/// Transforms the anchor position.
	/// Strictly for use with NGUI, takes a vector and the
	/// anchor that vector is in (baseAnchor) and transforms
	/// that position to coordinates as if the position were
	/// in the target anchor. Note: NGUI's coordinate system is (0,0) at top-right
	/// and (nativeWidht, nativeHeight) at bottom-left
	/// </summary>
	/// <returns>The new position in target anchor.</returns>
	/// <param name="position">Position.</param>
	/// <param name="baseAnchor">Base anchor.</param>
	/// <param name="targetAnchor">Target anchor.</param>
	public Vector3 TransformAnchorPosition(Vector3 position, InterfaceAnchors baseAnchor, InterfaceAnchors targetAnchor){
		// no need to do any transforming if the two anchors are the same
		if(baseAnchor == targetAnchor)
			return position;
		
		Vector3 transformedPosition = position;
		
		switch(targetAnchor){
		case InterfaceAnchors.Center:
			transformedPosition = ConvertToAnchorCenter(position, baseAnchor);
			break;
		case InterfaceAnchors.Top:
			transformedPosition = ConvertToAnchorTop(position, baseAnchor);
			break;
		case InterfaceAnchors.BottomRight:
			transformedPosition = ConvertToAnchorBottomRight(position, baseAnchor);
			break;
		default:
			Debug.LogError("Sorry, anchor not supported yet");
			break;
		}
		
		return transformedPosition;
	}

	private Vector3 ConvertToAnchorBottomRight(Vector3 position, InterfaceAnchors baseAnchor){
		Vector3 transformedPosition = position;

		switch(baseAnchor){
		case InterfaceAnchors.Center:
			transformedPosition.x -= nativeWidth / 2;
			transformedPosition.y += nativeHeight / 2;
			break;
		default:
			Debug.LogError("Sorry not implemented yet");
			break;
		}
		
		return transformedPosition;	
	}
	
	private Vector3 ConvertToAnchorCenter(Vector3 position, InterfaceAnchors baseAnchor){
		Vector3 transformedPosition = position;
		
		switch(baseAnchor){
		case InterfaceAnchors.BottomLeft:
			transformedPosition.x -= nativeWidth / 2;
			transformedPosition.y -= nativeHeight / 2;
			break;
		case InterfaceAnchors.Bottom:
			transformedPosition.y -= nativeHeight / 2;
			break;
		default:
			Debug.LogError("Sorry not implemented yet.");
			break;
		}
		
		return transformedPosition;		
	}
	
	private Vector3 ConvertToAnchorTop(Vector3 position, InterfaceAnchors baseAnchor){
		Vector3 vTransformed = position;
		
		switch(baseAnchor){
		case InterfaceAnchors.Center:
				 //vTransformed.x += nativeWidth / 2; //Jason: This doesn't make sense for center to top conversion
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
			Debug.LogError("Sorry not implemented yet.");
			break;
		}
		
		return vTransformed;		
	}	
}
