using UnityEngine;
using System.Collections;

/*
	Advair switch (Advair Step 2).

	This listens to the user's touch input, and rotates the part to follow the user's touch.

	If the part reaches its target rotation (ie. finalPosition), the step is completed.

	Unlike Advair Step 1, the switch bounces back to its original position regardless
	of whether the target was reached.

	The part only rotates in one direction, thanks to PreventAntiClockwiseRotation().
*/
public class InhalerSwitch : MonoBehaviour
{
	public GameObject inhalerSwitchArrow;
	Vector2 previousTouchPosition = Vector2.zero;
	Vector2 currentTouchPosition = Vector2.zero;
	bool previousFrameTouchDown = false;
	bool completelyOpened = false;
	Vector3 maxPosition =  new Vector3(0,0,30);

	void Update()
	{
		if (Input.touchCount == 0) { // if not touching screen
			ResetTouch();
			SnapBack();
		}
		else if(Input.touchCount > 0 && !completelyOpened){
			if (InhalerLogic.Instance.CurrentStep != 2){
				return;
			}
			Touch touch = Input.GetTouch(0);
			float touchx = touch.position.x;
			float touchy = touch.position.y;
			Vector2 touchPos = new Vector2( touchx,Screen.height - touchy);

			// first touch. (GetMouseButtonDown(0) only happens once)
			if (Input.GetMouseButtonDown(0) && isTouchingObject(touch) && !previousFrameTouchDown) {
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

			Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
			Vector2 screenPositionXY = new Vector2(screenPosition.x, screenPosition.y);
			Vector2 previousPositionVector = previousTouchPosition - screenPositionXY;
			Vector2 currentPositionVector = currentTouchPosition - screenPositionXY;

			if (previousPositionVector != -currentPositionVector && previousFrameTouchDown)
			{
				float rotationAmount = ReturnSignedAngleBetweenVectors(previousPositionVector,
					currentPositionVector);

				transform.RotateAroundLocal(Vector3.forward, rotationAmount *  Time.deltaTime);
				PreventAntiClockwiseRotation();
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

		int layer = 1 << 9; // InhalerSwitch is on layer 9
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
		if (transform.localRotation.z < 0){
			transform.localRotation = Quaternion.identity;
		}
	}

	void CheckIfCompletelyOpened(){
		if (transform.localEulerAngles.z >= 30){
			transform.localEulerAngles = maxPosition;
			completelyOpened = true;
			RemoveArrowAnimation();
			audio.Play();
			if (InhalerLogic.Instance.IsCurrentStepCorrect(2)){
				InhalerLogic.Instance.NextStep();
			}
		}
	}

	void RemoveArrowAnimation(){
		if (inhalerSwitchArrow){
			Destroy(inhalerSwitchArrow);
		}
	}

	void SnapBack(){

		if (transform.localRotation.z > 0){
			float rotationAmount = -5;
			transform.RotateAroundLocal(Vector3.forward, rotationAmount * Time.deltaTime);
		}
	}


}