using UnityEngine;
using System.Collections;

/*
    Inheriting this class will make the game object rotates by x angle around the z
    when finger drags from start to end position
*/
public class AdvairDragToRotate : MonoBehaviour {
    public GameObject hintArrow;
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
            if(completelyOpened) return; //Terminate touch right away if object already rated to finalPosition
            switch(touch.phase){
                case TouchPhase.Began:
                    //Condition that terminate touch
                    if(!IsTouchingObject(touch)) return;

                    //Store the vector of the first touch
                    startTouchPos = touch.position;
                    startVector = startTouchPos - centerPos;
                break;
                case TouchPhase.Moved:
                    //Conditions that terminate the touch
                    //Swipping to the left or finger is no longer touching the object will moving
                    if(touch.position.x < startTouchPos.x || !IsTouchingObject(touch)) return;

                    //Calculate the vector of the current touch position
                    Vector2 currentTouchPos = touch.position;
                    Vector2 currentVector = currentTouchPos - centerPos;

                    //Get the angle between the start vector and the current vector
                    float absAngle = ReturnSignedAngleBetweenVectors(startVector, currentVector);
                    transform.localEulerAngles = new Vector3(0, 0, absAngle); //Set the transform to the angle
                    CheckIfCompletelyOpened();
                break;
                case TouchPhase.Ended:
                    if(!completelyOpened) SnapBack();
                break;
            }
        }
    }

    //Cast a ray to test if the touch position is ontop of the object
    private bool IsTouchingObject(Touch touch){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;

        bool retVal = false;
        if (Physics.Raycast (ray, out hit, maskLayer)) {
            if(hit.collider.gameObject == this.gameObject){
                retVal = true;
            }
        }
        return retVal;
    }

    private float ReturnSignedAngleBetweenVectors(Vector2 vectorA, Vector2 vectorB)
    {
        // the vector that we want to measure an angle from
        /* some vector that is not Vector3.up */
        Vector3 referenceForward = new Vector3(vectorA.x, vectorA.y, 0);
         
        // the vector perpendicular to referenceForward (90 degrees clockwise)
        // (used to determine if angle is positive or negative)
        Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
         
        // the vector of interest
        /* some vector that we're interested in */
        Vector3 newDirection = new Vector3(vectorB.x, vectorB.y, 0);
         
        // Get the angle in degrees between 0 and 180
        float angle = Vector3.Angle(newDirection, referenceForward);
         
        // Determine if the degree value should be negative.  Here, a positive value
        // from the dot product means that our vector is the right of the reference vector   
        // whereas a negative value means we're on the left.
        float sign = (Vector3.Dot(newDirection, referenceRight) > 0.0f) ? -1.0f: 1.0f;
         
        float finalAngle = sign * angle;
        return finalAngle;
    }

    //Check if the inhaler body has been snap to the right position
    private void CheckIfCompletelyOpened(){
        if(transform.localEulerAngles.z >= finalPosition.z){ //151 is the final rotation value
            transform.localEulerAngles = finalPosition;
            completelyOpened = true;
            if(InhalerLogic.Instance.IsCurrentStepCorrect(advairStepID)){
                InhalerLogic.Instance.NextStep();
                collider.enabled = false;
            }
            RemoveArrowAnimation();
        }
    }

    //Remove the hint arrow animation
    private void RemoveArrowAnimation(){
        if(hintArrow != null){
            Destroy(hintArrow);
        }
    }

    //Return transform to the original rotation 
    private void SnapBack(){
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
