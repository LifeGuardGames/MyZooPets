using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour{

	private bool zoomed = false;
	private Vector3 initPosition;	// Default position: 0, 5.7, -23
	private Vector3 initFaceDirection;

	private GameObject shelf;
	// private Vector3 shelfFinalPosition = new Vector3 (10.7f,1.6f,6.6f);
	private Vector3 shelfFinalPosition;
	// private Vector3 shelfFinalFaceDirection = new Vector3(7.34f,90.11f,359.62f);
	private Vector3 shelfFinalFaceDirection = new Vector3(0,353.8f, 0);

	// private Vector3 petSideFinalPosition = new Vector3(3f, 1.3f, -15f);
	// private Vector3 petSideFinalPosition = new Vector3(4.83f, 8.6f, 12.64f);
	private Vector3 petSideFinalPosition;
	private Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

	private Vector3 gameboyFinalPosition = new Vector3(-11.9f, -1.6f, -1.4f);
	private Vector3 gameboyFinalDirection = new Vector3(27f, 0, 1.35f);

	// private Vector3 slotMachineFinalPosition = new Vector3(-9.66f, 9.95f, 32.58f);
	private GameObject slotMachine;
	private Vector3 slotMachineFinalPosition;
	private Vector3 slotMachineFinalDirection = new Vector3(17.8f, 20.47f, 0f);

	private GameObject realInhaler;
	private Vector3 realInhalerFinalPosition;
	private Vector3 realInhalerFinalDirection = new Vector3(36f, 0, 0);

	private GameObject teddyInhaler;
	private Vector3 teddyInhalerFinalPosition;
	private Vector3 teddyInhalerFinalDirection = new Vector3(15.54f, 0, 0);

	private Vector3 petCameraOffset = new Vector3(4.83f, 8.253f, -10.36f); // use this whenever changing petSideFinalPosition
	private Vector3 realInhalerCameraOffset = new Vector3(0.69f, 2.91f, -4.31f); // use this whenever changing realInhalerFinalPosition
	private Vector3 teddyInhalerCameraOffset = new Vector3(0.99f, 2.02f, -10.36f); // use this whenever changing teddyInhalerFinalPosition
	private Vector3 slotMachineCameraOffset = new Vector3(-0.2f, 9.95f, -8.2f); // use this whenever changing slotMachineFinalPosition
	private Vector3 shelfCameraOffset = new Vector3(-39.4f, -0.29f, 2.08f); // use this whenever changing shelfFinalPosition
	// this way, the camera will always go to the pet

	private GameObject spritePet;

	private bool isCameraMoving = false;

	private bool isLoadLevel = false;
	private string levelToLoad;

	private bool isEnterMode = false;

	public void Init(){
		//Camera move is used in multiple scenes so it needs to know what specific
		//gameobjects to load at diff scenes
		spritePet = GameObject.Find("SpritePet");

		if(Application.loadedLevelName == "NewBedRoom"){
			slotMachine = GameObject.Find("SlotMachine");
			realInhaler = GameObject.Find("RealInhaler");
			teddyInhaler = GameObject.Find("TeddyInhaler");
			shelf = GameObject.Find("Shelf");
		}else if(Application.loadedLevelName == "Yard"){

		}

		initPosition = gameObject.transform.position;
		// initFaceDirection = new Vector3(15.54f, 0, 0);
		initFaceDirection = gameObject.transform.eulerAngles;
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
	    		zoomed = true;
				LockCameraMove();

				shelfFinalPosition = shelf.transform.position + shelfCameraOffset;
	    		CameraTransformEnterMode(shelfFinalPosition,shelfFinalFaceDirection, 1.0f);
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

				petSideFinalPosition = spritePet.transform.position + petCameraOffset;
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

	public void SlotMachineZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				// SOMETHING HERE
			}
			else{
				zoomed = true;
				LockCameraMove();
				slotMachineFinalPosition = slotMachine.transform.localPosition + slotMachineCameraOffset;
				CameraTransformLoadLevel(slotMachineFinalPosition, slotMachineFinalDirection, 2f, "SlotMachineGame");
			}
		}
	}
	public void RealInhalerZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				// SOMETHING HERE
			}
			else{
				zoomed = true;
				LockCameraMove();
				realInhalerFinalPosition = realInhaler.transform.localPosition + realInhalerCameraOffset;
				CameraTransformLoadLevel(realInhalerFinalPosition, realInhalerFinalDirection, 2f, "InhalerGamePet");
			}
		}
	}
	public void TeddyInhalerZoomToggle(){
		if(!isCameraMoving){
			if(zoomed){
				// SOMETHING HERE
			}
			else{
				zoomed = true;
				LockCameraMove();
				teddyInhalerFinalPosition = teddyInhaler.transform.localPosition + teddyInhalerCameraOffset;
				CameraTransformLoadLevel(teddyInhalerFinalPosition, teddyInhalerFinalDirection, 2f, "InhalerGameTeddy");
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
		LeanTween.moveLocal(gameObject, newPosition, time, optional);
		LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
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
		LeanTween.moveLocal(gameObject, newPosition, time, optional);
		LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
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
		LeanTween.moveLocal(gameObject, newPosition, time, optional);
		LeanTween.rotateLocal(gameObject, newDirection, time, optional2);
	}

	private void ZoomOutMove(float time){
		CameraTransformExitMode(initPosition,initFaceDirection, time);
	}
}
