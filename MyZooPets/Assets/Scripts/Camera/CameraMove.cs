using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour{
	
	private bool zoomed = false;
	private Vector3 initPosition;	// Default position: 0, 5.7, -23
	private Vector3 initFaceDirection;
	
	private Vector3 shelfFinalPosition = new Vector3 (10.7f,1.6f,6.6f);
	private Vector3 shelfFinalFaceDirection = new Vector3(7.34f,90.11f,359.62f);
	
	private Vector3 petSideFinalPosition = new Vector3(2.5f, 2f, -15f);
	private Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);
	
	private Vector3 gameboyFinalPosition = new Vector3(-11.9f, -1.6f, -1.4f);
	private Vector3 gameboyFinalDirection = new Vector3(27f, 0, 1.35f);
	
	private bool isCameraMoving = false;
	
	private bool tryLoadLevel = false;
	private string levelToLoad;
	
	void Start(){
		initPosition = gameObject.transform.position;
		initFaceDirection = new Vector3(15.54f, 0, 0);
	}
	
	// Called from ClickManager
	// TODO toggle scheme using toggle, might want to set to definitive? (potential for transition bugs)
	public void ShelfZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				ZoomOutMove();
				zoomed = false;
				LockCameraMove();
			}
			else{		
	    		CameraTransform(shelfFinalPosition,shelfFinalFaceDirection, 1.0f);
	    		zoomed = true;
				LockCameraMove();
			}
		}
	}
	
	public void PetSideZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				ZoomOutMove();
				zoomed = false;
				LockCameraMove();
			}
			else{
				zoomed = true;
				LockCameraMove();
	    		CameraTransform(petSideFinalPosition,petSideFinalFaceDirection, 0.8f);
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
				CameraTransformLoadLevel(gameboyFinalPosition, gameboyFinalDirection, 2f, "InhalerGameBothInhalers");
			}
		}
	}
	
	public void LockCameraMove(){
		isCameraMoving = true;
	}
	
	public void UnlockCameraMove(){
		isCameraMoving = false;
		if(tryLoadLevel && (levelToLoad != null)){
			tryLoadLevel = false;
			Application.LoadLevel(levelToLoad);
		}
	}
	
	// Transforms camera
	public void CameraTransform(Vector3 newPosition, Vector3 newDirection, float time){
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		LeanTween.rotate(gameObject, newDirection, time, optional);
	}
	
	// Same as CameraTransform but tries to load a scene after the transform has completed
	public void CameraTransformLoadLevel(Vector3 newPosition, Vector3 newDirection, float time, string level){
		tryLoadLevel = true;
		levelToLoad = level;
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "UnlockCameraMove");
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		LeanTween.rotate(gameObject, newDirection, time, optional);
	}
	
	private void ZoomOutMove(){
		CameraTransform(initPosition,initFaceDirection, 1.0f);
	}
}
