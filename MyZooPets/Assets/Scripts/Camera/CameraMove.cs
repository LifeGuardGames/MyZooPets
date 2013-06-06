using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	public float smooth = 1.5f;
	
	private bool zoomed = false;
	private Vector3 initPosition;
	private Vector3 initFaceDirection;
	private Vector3 finalPosition;
	private Vector3 finalFaceDirection;
	
	void Start () {
		initPosition = gameObject.transform.position;
		initFaceDirection = new Vector3(15.54f,0,0);
		
		finalPosition = new Vector3 (10.7f,1.6f,6.6f);
		finalFaceDirection = new Vector3(7.34f,90.11f,359.62f);
	}
	
	void Update () {
		
		Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(myRay,out hit))
		{
			if(hit.collider.name == "room_shelf"&&Input.GetMouseButtonUp(0))
			{
				//print("You clicked shelf!");
				if(zoomed)
				{
					ZoomOutMove();
					zoomed = false;
				}
				else
				{		
	        		CameraTransform(finalPosition,finalFaceDirection);
    	    		zoomed = true;
				}	
			}
		}
		
		
		
//		if (Input.GetKeyUp(KeyCode.Space))
//		{
//			if(zoomed)
//			{
//				ZoomOutMove();
//				zoomed = false;
//			}
//			else
//			{		
//	          CameraTransform(finalPosition,finalFaceDirection);
//    	      zoomed = true;
//			}
//		}
	}
	
	void CameraTransform (Vector3 newPosition, Vector3 newDirection){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, smooth, optional);
		LeanTween.rotate(gameObject, newDirection, smooth, optional);
	}
	
	void ZoomOutMove(){
		CameraTransform(initPosition,initFaceDirection);
	}
}


