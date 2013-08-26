using UnityEngine;
using System.Collections;

/*
    Inheriting this class will make the game object rotates by x angle around the z
    when finger drags from start to end position
*/
public class AdvairDragToRotate : MonoBehaviour {
    private Vector3 centerWorlPos; //Position of the center of the Inhaler game object
    private Vector2 centerPos;
    private Vector2 startTouchPos; //Position of touch in TouchPhase.Began
    private Vector2 startVector;
    private bool completelyOpened; //True: transform reaches its final position

    //Variables that need to be defined by child class
    protected Vector3 finalPosition; //the final position that the transform can be rotated to
    protected int advairStepID; //step id correlating to Inhaler Logic
    protected int maskLayer; //specify layer if GO is not on default layer

    protected virtual void Awake(){
        centerWorlPos = Camera.main.WorldToScreenPoint(transform.position);     
        centerPos = new Vector2(centerWorlPos.x, centerWorlPos.y);
    }

    protected virtual void Update()
    {
        if(Input.touchCount > 0){
            Touch touch = Input.touches[0];
            if(completelyOpened) return; //Terminate touch right away if object already rotate to finalPosition
            switch(touch.phase){
                case TouchPhase.Began:
                    //Condition that terminate touch
                    if(!InhalerUtility.IsTouchingObject(touch, gameObject, maskLayer)) return;

                    //Store the vector of the first touch
                    startTouchPos = touch.position;
                    startVector = startTouchPos - centerPos;
                break;
                case TouchPhase.Moved:
                    //Calculate the vector of the current touch position
                    Vector2 currentTouchPos = touch.position;
                    Vector2 currentVector = currentTouchPos - centerPos;
                    //Get the angle between the start vector and the current vector
                    float signedAngle = ReturnSignedAngleBetweenVectors(startVector, currentVector);
                    
                    //Conditions that terminate the touch
                    //only want object to rotate right while finger is touching object
                    if(signedAngle >= 0 || !InhalerUtility.IsTouchingObject(touch, gameObject, maskLayer)) return; 
                    transform.localEulerAngles = new Vector3(0, 0, signedAngle); //Set the transform to the angle
                    CheckIfCompletelyOpened();
                break;
                case TouchPhase.Ended:
                    if(!completelyOpened) SnapBack();
                break;
            }
        }
    }

    private float ReturnSignedAngleBetweenVectors(Vector2 startVector, Vector2 currentVector)
    {
        float angle = Mathf.Atan2(currentVector.y, currentVector.x) - Mathf.Atan2(startVector.y, startVector.x);
        return angle * Mathf.Rad2Deg;
    }

    //Check if the inhaler body has been snap to the right position
    private void CheckIfCompletelyOpened(){
        if(transform.localEulerAngles.z <= finalPosition.z){
            transform.localEulerAngles = finalPosition;
            completelyOpened = true;
            if(InhalerLogic.Instance.IsCurrentStepCorrect(advairStepID)){
                InhalerLogic.Instance.NextStep();
                collider.enabled = false;
            }
        }
    }

    //Return transform to the original rotation 
    private void SnapBack(){
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
