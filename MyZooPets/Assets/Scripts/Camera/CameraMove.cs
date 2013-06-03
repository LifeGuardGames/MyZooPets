using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	private Vector3 initPosition;
	private Vector3 initFaceDirection;
	private Vector3 finalPosition;
	private Vector3 finalFaceDirection;
	
	void Start () {
		initPosition = gameObject.transform.position;
	}
	
	void Update () {
	
	}
	
	void CameraTransform (Vector3 newPosition){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(gameObject, newPosition, 0.5f, optional);
	}
	
	void ZoomOutMove(){
		CameraTransform(initPosition);
	}
}
