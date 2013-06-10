using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	public float smooth = 1.5f;
	
	private bool zoomed = false;
	private Vector3 initPosition;
	private Vector3 initFaceDirection;
	private Vector3 shelfFinalPosition;
	private Vector3 shelfFinalFaceDirection;
	
	void Start () {
		initPosition = gameObject.transform.position;
		initFaceDirection = new Vector3(15.54f,0,0);
		
		shelfFinalPosition = new Vector3 (10.7f,1.6f,6.6f);
		shelfFinalFaceDirection = new Vector3(7.34f,90.11f,359.62f);
	}
	
	// Called from ClickManager
	public void ShelfZoomToggle(){
		if(zoomed)
		{
			ZoomOutMove();
			zoomed = false;
		}
		else
		{		
    		CameraTransform(shelfFinalPosition,shelfFinalFaceDirection);
    		zoomed = true;
		}	
	}
	
	public void PetZoomToggle(){
		if(zoomed)
		{
			ZoomOutMove();
			zoomed = false;
		}
		else
		{		
    		CameraTransform(shelfFinalPosition,shelfFinalFaceDirection);
    		zoomed = true;
		}	
	}
	
	private void CameraTransform (Vector3 newPosition, Vector3 newDirection){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, smooth, optional);
		LeanTween.rotate(gameObject, newDirection, smooth, optional);
	}
	
	private void ZoomOutMove(){
		CameraTransform(initPosition,initFaceDirection);
	}
}


