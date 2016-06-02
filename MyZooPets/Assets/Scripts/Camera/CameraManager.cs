using UnityEngine;
using System.Collections;

/// <summary>
/// Camera manager.
/// Two parts to this class one camera movement and one that handles camera utility for screen conversion
/// </summary>
public class CameraManager : Singleton<CameraManager>{

	public Camera cameraMain;
	public Camera CameraMain{
		get{ return cameraMain; }
	}

	#region Camera Move Variables
	// Default position/rotation of the camera
	protected Vector3 initPosition;	// Default position: 0, 5.7, -23
	protected Vector3 initFaceDirection;	

	public PanToMoveCamera scriptPan;
	public PanToMoveCamera PanScript{
		get { return scriptPan; }
	}

	private bool isZoomed;		// Is the camera zoomed in
	private bool isZoomingAux;	// Intermediate check to track if zoomed in or out when zoom is finished
	private bool isMoving;		// Is the camera moving currently
	public bool IsMoving{
		get { return isMoving; }
		set { isMoving = value; }
	}

	public delegate void Callback(); // Used for notification entry
	public Callback FinishCameraMoveCallback;
	#endregion

	#region Utility Variables
	public float ratioX;
	public float ratioY;

	private int nativeWidth;
	public int NativeWidth{
		get { return nativeWidth;}
	}
	
	private int nativeHeight;
	public int NativeHeight{
		get { return nativeHeight; }
	}
	#endregion

	void Awake(){
		// Native height is a fixed constant that we define for NGUI
		nativeHeight = Constants.GetConstant<int>("NativeHeight");
		ratioY = nativeHeight / (Screen.height * 1.0f);
		
		// Native width is not a constant -- it is a thing created by NGUI based on the height
		nativeWidth = (int)(Screen.width * ratioY);
		ratioX = nativeWidth / (Screen.width * 1.0f);
	}	

	#region Camera Move Funtions
	/// <summary>
	/// Returns true if the camera is currently moving.
	/// NOTE: I used to check for a lean tween, but we
	/// sometimes move the camera without using lean tween.
	/// So now I just look at the X of the camera and see if
	/// is where it should be.
	/// </summary>
	/// <returns><c>true</c> if this camera is moving; otherwise, <c>false</c>.</returns>
	public bool IsCameraMoving(){
		if(scriptPan == null){	// Hack for screens that do not have panning
			return false;
		}

		GameObject goParent = transform.parent.gameObject;
		float targetX = scriptPan.partitionOffset * scriptPan.currentLocalPartition;
		float fX = goParent.transform.position.x;
		
		// Check if the camera is moving horizontally or zooming
		bool auxMoving = (targetX != fX) || isMoving;
		return auxMoving;
	}

	/// <summary>
	/// Moves the camera to a target position with a 
	/// target rotation over a set time.
	/// NOTE: If this function can ever "fail", be sure to
	/// check ZoomHelper because it assumes this function
	/// will always work.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	/// <param name="time">Time.</param>
	/// <param name="cameraDoneCallback">Callback function on camera done moving</param>
	public void ZoomToTarget(Vector3 position, Vector3 rotation, float time, CameraManager.Callback cameraDoneCallback){
		// Before zooming, cache the camera position
		initPosition = gameObject.transform.position;
		initFaceDirection = gameObject.transform.eulerAngles;
		
		isZoomed = true;
		isZoomingAux = true;	// Set the flag to true
		isMoving = true;
		
		MoveCamera(position, rotation, time, cameraDoneCallback);
	}

	/// <summary>
	/// Zooms camera back to its original position over time.
	/// </summary>
	public void ZoomOutMove(){
		ZoomOutMove(.3f);
	}

	public void ZoomOutMove(float time){
		if(isZoomed){
			isZoomingAux = false; // Set the flag to false
			isMoving = true;
			MoveCamera(initPosition, initFaceDirection, time, null);
		}
	}

	private void MoveCamera(Vector3 position, Vector3 rotation, float time, CameraManager.Callback cameraDoneCallback){
		// Make sure to subtract the camera's parent's position from the vPos because the parent moves as the partitions pan
		position -= gameObject.transform.parent.position;
		
		// If the incoming object isn't null, set up a callback
		if(cameraDoneCallback != null){
			FinishCameraMoveCallback = cameraDoneCallback;	// Cache the callback
		}

		// Kick off the move and rotation tweens
		LeanTween.moveLocal(gameObject, position, time).setEase(LeanTweenType.easeInOutQuad).setOnComplete(MoveCameraDone);
		LeanTween.rotateLocal(gameObject, rotation, time).setEase(LeanTweenType.easeInOutQuad);	
	}

	private void MoveCameraDone(){
		isMoving = false;
		isZoomed = isZoomingAux ? true : false;	// Determine if zoomed in or out based on flag

		if(FinishCameraMoveCallback != null){
			FinishCameraMoveCallback();			// Call the callback
			FinishCameraMoveCallback = null;	// Unassign the callback once it is called
		}
	}
	#endregion

	#region Utility Functions
	public float GetRatioDifference(){
		return ratioY;
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
		case InterfaceAnchors.TopLeft:
			transformedPosition = ConvertToAnchorTopLeft(position, baseAnchor);
			break;
		case InterfaceAnchors.TopRight:
			transformedPosition = ConvertToAnchorTopRight(position, baseAnchor);
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
		case InterfaceAnchors.BottomLeft:
			transformedPosition.x -= nativeWidth;
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
		Vector3 transformedPosition = position;
		
		switch(baseAnchor){
		case InterfaceAnchors.Center:
				 //vTransformed.x += nativeWidth / 2; //Jason: This doesn't make sense for center to top conversion
													 // so i commented it out. center to top has the same x only the
													 //Y change...not sure why this was here in the beginning. hope i
													 //didn't break more code
			transformedPosition.y -= nativeHeight / 2;
			break;
		case InterfaceAnchors.BottomLeft:
			transformedPosition.x -= nativeWidth / 2;
			transformedPosition.y -= nativeHeight;
			break;
		case InterfaceAnchors.TopLeft:
			transformedPosition.x -= nativeWidth / 2;
			break;	
		default:
			Debug.LogError("Sorry not implemented yet.");
			break;
		}
		
		return transformedPosition;		
	}

	private Vector3 ConvertToAnchorTopRight(Vector3 position, InterfaceAnchors baseAnchor){
		Vector3 transformedPosition = position;

		switch(baseAnchor){
		case InterfaceAnchors.Center:
			transformedPosition.x -= nativeWidth / 2;
			transformedPosition.y -= nativeHeight / 2;
			break;
		case InterfaceAnchors.BottomLeft:
			transformedPosition.x -= nativeWidth;
			transformedPosition.y -= nativeHeight;
			break;
		default:
			Debug.LogError("Sorry not implemented yet.");
			break;
		}

		return transformedPosition;
	}

	private Vector3 ConvertToAnchorTopLeft(Vector3 position, InterfaceAnchors baseAnchor){
		Vector3 transformedPosition = position;

		switch(baseAnchor){
		case InterfaceAnchors.Center:
			transformedPosition.x += nativeWidth / 2;
			transformedPosition.y -= nativeHeight / 2;
			break;
		case InterfaceAnchors.BottomLeft:
			transformedPosition.y -= nativeHeight;
			break;
		default:
			Debug.LogError("Sorry not implemented yet.");
			break;
		}

		return transformedPosition;
	}
	#endregion
}
