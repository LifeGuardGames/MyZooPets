using UnityEngine;
using System;
using System.Collections;

/*
    Rescue Inhaler Button / Prescription (Rescue Step 5).

    This listens for the user's touch on the button (prescription bottle) on the rescue inhaler (the small one in front of the pet's mouth).

    If the switch is tapped, it will go down, and the game will move on to the next step.
*/
public class RescueSwitch : MonoBehaviour {
    private int gameStepID = 5;
    private Vector2 startTouchPos; //Position of touch in TouchPhase.Began    
    private int minSwipeDistance = 20; //How far the swipe needs to be before it's recognized
    private bool firstTouchOnObject = false; //User needs to touch the object first before
                                            //touch events can be handled 

    void Start(){
        InhalerLogic.OnNextStep += CheckAndEnable;
        Disable();
    }                                        

    void OnDestroy(){
        InhalerLogic.OnNextStep -= CheckAndEnable;
    }

    void Update()
    {
        if(InhalerLogic.Instance.CurrentStep != gameStepID) return;

        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            switch(touch.phase){
                case TouchPhase.Began:
                    //Condition that terminate touch
                    if(!InhalerUtility.IsTouchingObject(touch, gameObject)) return;

                    firstTouchOnObject = true;
                    startTouchPos = touch.position;
                break;
                case TouchPhase.Moved:
                    //Condition that terminate touch
                    if(!firstTouchOnObject) return;

                    if(IsDraggingDown(touch)){ //if swiping down check if the step is correct
                        if(InhalerLogic.Instance.IsCurrentStepCorrect(gameStepID)){
                            PrescriptionSwipedAnimation();
                            firstTouchOnObject = false;
                        }
                    }
                break;
                case TouchPhase.Ended:
                    //Condition that terminate touch
                    if(!firstTouchOnObject) return;
                    firstTouchOnObject = false;
                break;
            }
        }
    }

    //Event Listener
    private void CheckAndEnable(object sender, EventArgs args){
        if(gameStepID == InhalerLogic.Instance.CurrentStep &&
            InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            Renderer[] components = transform.parent.GetComponentsInChildren<Renderer>();
            foreach(Renderer red in components){
                red.enabled = true;
            }
        }
    }

    //Hide the small rescue inhaler
    private void Disable(){
        Renderer[] components = transform.parent.GetComponentsInChildren<Renderer>();
        foreach(Renderer red in components){
            red.enabled = false;
        }
    }

    private void PrescriptionSwipedAnimation(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "AnimationCallBack");
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        LeanTween.move(gameObject, new Vector3(x,y - 1.0f,z), 0.5f, optional);
    }

    //Disable and move on to next step after animation is finished
    private void AnimationCallBack(){
        InhalerLogic.Instance.NextStep();
        Disable();
    }

    //Handles swipe down gesture
    private bool IsDraggingDown(Touch touch){
        bool retVal = false;
        if (startTouchPos.y - touch.position.y > minSwipeDistance){
            retVal = true;;
        }
        return retVal;
    }
}