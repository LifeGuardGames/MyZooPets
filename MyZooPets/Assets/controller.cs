/////
///// Rotate tutorial.
///// Copyright 2011 revelopment.co.uk
///// Created by Carlos Revelo
///// 2011
/////
/////
/////
/////
//
//using UnityEngine;
//using System.Collections;
//
//public class controller : MonoBehaviour {
//
//
////	private float x = transform.rotation.x;
////	private float y = transform.rotation.y;
////	private float z = transform.rotation.z;
////
//
// [SerializeField]
//	float 	_speed = 1f;
//
//	bool _canRotate = false;
//
//	Transform _cachedTransform;
//
//	public bool CanRotate
//	{
//		get { return _canRotate; }
//
//		private set { _canRotate = value; }
//	}
//
//
//	void Start () {
//
//		//Make reference to transform
//		_cachedTransform = transform;
//
//	}
//
//
//
//	// Update is called once per frame
//	void Update () {
//
//		if(Input.touchCount > 0)
//		{
//			Touch touch = Input.GetTouch(0);
//
//			//Switch through touch events
//			switch(Input.GetTouch(0).phase)
//
//			{
//				case TouchPhase.Began:
//					if(VerifyTouch(touch))
//						CanRotate = true;
//				break;
//
//				case TouchPhase.Moved:
//					if(CanRotate)
//						RotateObject(touch);
//				break;
//				case TouchPhase.Ended:
//					CanRotate = false;
//					transform.rotation = (new Vector3(0,0,0));
//				break;
//
//			}
//
//		}
//
//	}
//
//	///
//	/// Verifies the touch.
//	///
//	///
//	/// The touch.
//	///
//	///
//	/// If set to true touch.
//	///
//	bool VerifyTouch(Touch touch)
//	{
//		Ray ray = Camera.main.ScreenPointToRay(touch.position);
//        	RaycastHit hit ;
//
//		//Check if there is a collider attached already, otherwise add one on the fly
//		if(collider == null)
//			gameObject.AddComponent(typeof(BoxCollider));
//
//       		if (Physics.Raycast (ray, out hit))
//		{
//			if(hit.collider.gameObject == this.gameObject)
//				return true;
//		}
//		return false;
//	}
//
//
//
//	///
//	/// Rotates the object.
//	///
//	///
//	/// Touch.
//	///
//	void RotateObject(Touch touch)
//	{
//
//
////		_cachedTransform.Rotate(new Vector3(touch.deltaPosition.y, -touch.deltaPosition.x,0)*_speed, Space.World);
//		float xmove = touch.deltaPosition.x;
//		float ymove = touch.deltaPosition.y;
//
//		if(xmove < 0) xmove =0;
//		if(ymove >0 ) ymove =0;
//		ymove = Mathf.Abs(ymove);
//		//if(ymove < 0) ymove =0;
//		_cachedTransform.Rotate(0,0, xmove+ymove, Space.World);
//	}
//
//}





using UnityEngine;
using System.Collections;

public class controller : MonoBehaviour
{
	Vector2 previousTouchPosition = Vector2.zero;
	Vector2 currentTouchPosition = Vector2.zero;
	bool previousFrameTouchDown = false;
	bool dragStartedOnObject = false; // if the drag was first started on the object, instead of entering the object later on

	void Update()
	{
		if (Input.touchCount == 0) { // if not touching screen
			ResetTouch();
			dragStartedOnObject = false;
		}
		else if(Input.touchCount > 0){
			Touch touch = Input.GetTouch(0);
			if (!isTouchingObject(touch)){ // if finger has left object
				ResetTouch();
			}
			else {
				float touchx = touch.position.x;
				float touchy = touch.position.y;
				Vector2 touchPos = new Vector2( touchx,Screen.height - touchy);

				// first touch. (GetMouseButtonDown(0) only happens once)
				if (Input.GetMouseButtonDown(0) && !previousFrameTouchDown) {
					previousTouchPosition = touchPos;
					currentTouchPosition = touchPos;
					previousFrameTouchDown = true;
					dragStartedOnObject = true;
				}
				// finger was just dragged into the object
				// (condition: the first touch was on the object, but the finger left the object at some point)
				else if (dragStartedOnObject && Input.GetMouseButton(0) && !previousFrameTouchDown){
					previousTouchPosition = touchPos;
					currentTouchPosition = touchPos;
					previousFrameTouchDown = true;
				}

				// last touch was also inside object
				else if (Input.GetMouseButton(0) && previousFrameTouchDown)
				{
					previousTouchPosition = currentTouchPosition;
					currentTouchPosition = touchPos;
				}

				// not needed, since the check for touchCount == 0 will take care of this
				// else if (!Input.GetMouseButton(0))
				// {
				// 	previousFrameTouchDown = false;
				// }

				Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
				Vector2 screenPositionXY = new Vector2(screenPosition.x, screenPosition.y);
				Vector2 previousPositionVector = previousTouchPosition - screenPositionXY;
				Vector2 currentPositionVector = currentTouchPosition - screenPositionXY;

				if (previousPositionVector != -currentPositionVector && previousFrameTouchDown)
				{
					float rotationAmount = ReturnSignedAngleBetweenVectors(previousPositionVector,
						currentPositionVector);

					transform.RotateAroundLocal(Vector3.forward, rotationAmount * Time.deltaTime);
				}
			}
		}
	}

	bool isTouchingObject(Touch touch){
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
       	RaycastHit hit ;

		//Check if there is a collider attached already, otherwise add one on the fly
		if(collider == null){
			gameObject.AddComponent(typeof(BoxCollider));
		}

		if (Physics.Raycast (ray, out hit)) {
			if(hit.collider.gameObject == this.gameObject){
				return true;
			}
		}
		return false;
	}

	void ResetTouch(){
		previousTouchPosition = Vector2.zero;
		currentTouchPosition = Vector2.zero;
		previousFrameTouchDown = false;
	}

	private float ReturnSignedAngleBetweenVectors(Vector2 vectorA, Vector2 vectorB)
	{
		Vector3 vector3A = new Vector3(vectorA.x, vectorA.y, 0f);
		Vector3 vector3B = new Vector3(vectorB.x, vectorB.y, 0f);

		if (vector3A == vector3B)
			return 0f;

		// refVector is a 90cw rotation of vector3A
		Vector3 refVector = Vector3.Cross(vector3A, Vector3.forward);
		float dotProduct = Vector3.Dot(refVector, vector3B);

		if (dotProduct > 0)
			return -Vector3.Angle(vector3A, vector3B);
		else if (dotProduct < 0)
			return Vector3.Angle(vector3A, vector3B);
		else
			throw new System.InvalidOperationException("the vectors are opposite");
	}


}