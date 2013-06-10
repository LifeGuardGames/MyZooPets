using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour{
	
	private bool zoomed = false;
	private Vector3 initPosition;	// 0, 5.7, -23
	private Vector3 initFaceDirection;
	
	private Vector3 shelfFinalPosition = new Vector3 (10.7f,1.6f,6.6f);
	private Vector3 shelfFinalFaceDirection = new Vector3(7.34f,90.11f,359.62f);
	
	private Vector3 petSideFinalPosition = new Vector3(2.5f, 2f, -15f);
	private Vector3 petSideFinaleFaceDirection = new Vector3(15.54f, 0, 0);
	
	void Start(){
		initPosition = gameObject.transform.position;
		initFaceDirection = new Vector3(15.54f, 0, 0);
	}
	
	// Called from ClickManager
	public void ShelfZoomToggle(){
		if(zoomed){
			ZoomOutMove();
			zoomed = false;
		}
		else{		
    		CameraTransform(shelfFinalPosition,shelfFinalFaceDirection, 1.0f);
    		zoomed = true;
		}	
	}
	
	public void PetSideZoomToggle(){
		if(zoomed){
			ZoomOutMove();
			zoomed = false;
		}
		else{
    		CameraTransform(petSideFinalPosition,petSideFinaleFaceDirection, 0.8f);
    		zoomed = true;
		}	
	}
	
	private void CameraTransform (Vector3 newPosition, Vector3 newDirection, float time){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, time, optional);
		LeanTween.rotate(gameObject, newDirection, time, optional);
	}
	
	private void ZoomOutMove(){
		CameraTransform(initPosition,initFaceDirection, 1.0f);
	}
}
