using UnityEngine;
using System.Collections;

public abstract class CameraMove : MonoBehaviour{

	protected bool zoomed = false;
	protected Vector3 initPosition;	// Default position: 0, 5.7, -23
	protected Vector3 initFaceDirection;

	protected bool isCameraMoving = false; //True: camera moving, False: camera static

	protected bool isLoadLevel = false; //True: there's a level to be loaded, False: no level
	protected string levelToLoad; //name of the level that need to be loaded

	protected bool isEnterMode = false; //True: camera will zoom into the specified game object

	public bool IsCameraMoving{
		get{return isCameraMoving;}
	}

	public abstract void ZoomToggle(ZoomItem item);

	protected virtual void Start(){
        initPosition = gameObject.transform.position;
        initFaceDirection = gameObject.transform.eulerAngles;
	}

	public void ZoomOutMove(){
		ZoomOutMove(0.5f);
	}
	public void ZoomOutMove(float time){
		if (zoomed){
			CameraTransformExitMode(initPosition,initFaceDirection, time);
			zoomed = false;
			LockCameraMove();
		}
	}

	protected void LockCameraMove(){
		isCameraMoving = true;
	}

	// Mostly called on callback from camera move
	protected void UnlockCameraMove(){
		isCameraMoving = false;
		if(!isEnterMode){
			//call event listener here
			ClickManager.ReleaseModeLock();		// Only want to release the lock after camera done when exiting
		}
		if(isLoadLevel && (levelToLoad != null)){
			isLoadLevel = false;
			Application.LoadLevel(levelToLoad);
		}
		ClickManager.ReleaseClickLock();
	}

	// Transforms camera
	protected void CameraWorldTransformEnterMode(Vector3 newPosition, Vector3 newDirection, float time){
		isLoadLevel = false;
		isEnterMode = true;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");		// Callback here
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		// LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
		LeanTween.rotate(gameObject, newDirection, time, optional2);
	}

	// Transforms camera
	protected void CameraTransformEnterMode(Vector3 newPosition, Vector3 newDirection, float time){
		isLoadLevel = false;
		isEnterMode = true;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");		// Callback here
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.moveLocal(gameObject, newPosition, time, optional);
		LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
	}

	// Transforms camera
	protected void CameraTransformExitMode(Vector3 newPosition, Vector3 newDirection, float time){
		isLoadLevel = false;
		isEnterMode = false;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");		// Callback here
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.moveLocal(gameObject, newPosition, time, optional);
		LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
	}

	// Same as CameraTransform but tries to load a scene after the transform has completed
	protected void CameraTransformLoadLevel(Vector3 newPosition, Vector3 newDirection, float time, string level){
		isLoadLevel = true;
		isEnterMode = true;
		levelToLoad = level;
		Hashtable optional = new Hashtable();
		Hashtable optional2 = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional2.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.moveLocal(gameObject, newPosition, time, optional);
		LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
	}
}
