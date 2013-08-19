using UnityEngine;
using System.Collections;

/*
	Advair part that rotates (Advair Step 1).

	This is actually incorrectly named InhalerCap, but what is really is is that part of the
	Advair inhaler that rotates to reveal a switch and a mouthpiecek.

	This listens to the user's touch input, and rotates the part to follow the user's touch.

	If the part reaches its target rotation (ie. finalPosition), the step is completed, and it stays there.
	If it doesn't, it snaps back to its original position when the touch is released.

	The part only rotates in one direction, thanks to PreventAntiClockwiseRotation().
*/
public class InhalerCap : MonoBehaviour
{
	public GameObject inhalerCapArrow;
	Vector2 previousTouchPosition = Vector2.zero;
	Vector2 currentTouchPosition = Vector2.zero;
	bool previousFrameTouchDown = false;
	bool dragStartedOnObject = false; // if the drag was first started on the object, instead of entering the object later on
	bool completelyOpened = false;
	Vector3 finalPosition =  new Vector3(0,0,151);

	void Update()
	{
		/*
			if(Input.touchCount > 0){
	
			}
			Touch touch = Input.touches[0];
            switch (touch.phase) {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                case TouchPhase.Ended:

		*/
		if (Input.touchCount == 0) { // if not touching screen
			ResetTouch();
			dragStartedOnObject = false;
			if (!completelyOpened){
				SnapBack();
			}
		}
		else if(Input.touchCount > 0 && !completelyOpened){
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
					print(rotationAmount);
					transform.RotateAroundLocal(Vector3.forward, rotationAmount *  Time.deltaTime);
					PreventAntiClockwiseRotation();
				}
			}
			CheckIfCompletelyOpened();
		}
		PreventAntiClockwiseRotation();
	}

	bool isTouchingObject(Touch touch){
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
       	RaycastHit hit ;

		//Check if there is a collider attached already, otherwise add one on the fly
		if(collider == null){
			gameObject.AddComponent(typeof(BoxCollider));
		}

		int layer = 1 << 8;
		if (Physics.Raycast (ray, out hit, layer)) {
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

	void PreventAntiClockwiseRotation(){
		if (transform.rotation.z < 0){
			transform.rotation = Quaternion.identity;
		}
	}

	void CheckIfCompletelyOpened(){
		if (transform.eulerAngles.z >= 151){
			transform.eulerAngles = finalPosition;
			completelyOpened = true;
			if (InhalerLogic.Instance.IsCurrentStepCorrect(1)){
				InhalerLogic.Instance.NextStep();
				collider.enabled = false;
			}
			RemoveArrowAnimation();
		}
	}

	void RemoveArrowAnimation(){
		if (inhalerCapArrow){
			Destroy(inhalerCapArrow);
		}
	}

	void SnapBack(){

		if (transform.eulerAngles.z > 0){
			float rotationAmount = -10;
			transform.RotateAroundLocal(Vector3.forward, rotationAmount * Time.deltaTime);
		}
	}


}