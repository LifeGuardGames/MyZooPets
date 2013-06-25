using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour{

	private bool zoomed = false;
	private Vector3 initPosition;	// Default position: 0, 5.7, -23
	private Vector3 initFaceDirection;

	private Vector3 shelfFinalPosition = new Vector3 (10.7f,1.6f,6.6f);
	private Vector3 shelfFinalFaceDirection = new Vector3(7.34f,90.11f,359.62f);

	// private Vector3 petSideFinalPosition = new Vector3(3f, 1.3f, -15f);
	private Vector3 petSideFinalPosition = new Vector3(1.67f, 6.52f, 14.8f);
	private Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

	private Vector3 gameboyFinalPosition = new Vector3(-11.9f, -1.6f, -1.4f);
	private Vector3 gameboyFinalDirection = new Vector3(27f, 0, 1.35f);

	private bool isCameraMoving = false;

	private bool isLoadLevel = false;
	private string levelToLoad;

	private bool isEnterMode = false;

	void Start(){
		initPosition = gameObject.transform.position;
		initFaceDirection = new Vector3(15.54f, 0, 0);
	}

	// Called from ClickManager
	// TODO toggle scheme using toggle, might want to set to definitive? (potential for transition bugs)
	public void ShelfZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				ZoomOutMove(1.0f);
				zoomed = false;
				LockCameraMove();
			}
			else{
	    		CameraTransformEnterMode(shelfFinalPosition,shelfFinalFaceDirection, 1.0f);
	    		zoomed = true;
				LockCameraMove();
			}
		}
	}

	public void PetSideZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				ZoomOutMove(0.5f);
				zoomed = false;
				LockCameraMove();
			}
			else{
				zoomed = true;
				LockCameraMove();
	    		CameraTransformEnterMode(petSideFinalPosition,petSideFinalFaceDirection, 0.5f);
			}
		}
	}

	public void GameboyZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				// SOMETHING HERE
			}
			else{
				zoomed = true;
				LockCameraMove();
				// CameraTransformLoadLevel(gameboyFinalPosition, gameboyFinalDirection, 2f, "InhalerGameBothInhalers");
				CameraTransformLoadLevel(gameboyFinalPosition, gameboyFinalDirection, 2f, "InhalerGamePet");
			}
		}
	}

	public void LockCameraMove(){
		isCameraMoving = true;
	}

	// Mostly called on callback from camera move
	public void UnlockCameraMove(){
		isCameraMoving = false;
		if(!isEnterMode){
			ClickManager.ReleaseModeLock();		// Only want to release the lock after camera done when exiting
		}
		if(isLoadLevel && (levelToLoad != null)){
			isLoadLevel = false;
			Application.LoadLevel(levelToLoad);
		}
		ClickManager.ReleaseClickLock();
	}

	// Transforms camera
	public void CameraTransformEnterMode(Vector3 newPosition, Vector3 newDirection, float time){
		isLoadLevel = false;
		isEnterMode = true;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");		// Callback here
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		LeanTween.rotate(gameObject, newDirection, time, optional2);
	}

	// Transforms camera
	public void CameraTransformExitMode(Vector3 newPosition, Vector3 newDirection, float time){
		isLoadLevel = false;
		isEnterMode = false;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");		// Callback here
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		LeanTween.rotate(gameObject, newDirection, time, optional2);
	}

	// Same as CameraTransform but tries to load a scene after the transform has completed
	public void CameraTransformLoadLevel(Vector3 newPosition, Vector3 newDirection, float time, string level){
		isLoadLevel = true;
		isEnterMode = true;
		levelToLoad = level;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		LeanTween.rotate(gameObject, newDirection, time, optional2);
	}

	private void ZoomOutMove(float time){
		CameraTransformExitMode(initPosition,initFaceDirection, time);
	}
}
