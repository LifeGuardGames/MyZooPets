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
		
		finalPosition = new Vector3 (0,2,-9);
		finalFaceDirection = new Vector3(12,39,10);
	}
	
	void Update () {
		
		if (Input.GetKeyUp(KeyCode.Space))
		{
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
