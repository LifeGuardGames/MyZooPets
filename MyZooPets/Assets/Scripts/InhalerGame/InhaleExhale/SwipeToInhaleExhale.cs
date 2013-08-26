using UnityEngine;
using System;
using System.Collections;

/*
    Inhale.cs and Exhale.cs inherit from this class. It handles swipe up and swipe down
    touch gesture
*/
public class SwipeToInhaleExhale : MonoBehaviour {
    protected int gameStepID;
    protected Vector2 startTouchPos; //Position of touch in TouchPhase.Began    
    protected int minSwipeDistance = 80; //How far the swipe needs to be before it's recognized
    private bool firstTouchOnObject = false; //User needs to touch the object first before
                                            //touch events can be handled    

    // Use this for initialization
    protected virtual void Start () {
        InhalerLogic.OnNextStep += CheckAndEnable;
        Disable();
    }

    protected virtual void OnDestroy(){
        InhalerLogic.OnNextStep -= CheckAndEnable;
    }
    
    // Update is called once per frame
    void Update () {
        if(Input.touchCount > 0){
            Touch touch = Input.touches[0];
            switch(touch.phase){
                case TouchPhase.Began:
                    //Condition that terminate touch
                    if(!InhalerUtility.IsTouchingObject(touch, gameObject)) return;

                    //Store first touch position
                    firstTouchOnObject = true;
                    startTouchPos = touch.position;
                break;
                case TouchPhase.Moved:
                    //Condition that terminate touch
                    if(!firstTouchOnObject) return;

                    if(IsDragging(touch)){
                        //If swipe down and correct step. Move on to the next step
                        if(InhalerLogic.Instance.IsCurrentStepCorrect(gameStepID)){
                            // exhale.Play();
                            InhalerLogic.Instance.NextStep();
                            Disable();
                        }
                    }
                break;
                case TouchPhase.Ended:
                    if(!firstTouchOnObject) return;

                    firstTouchOnObject = false;
                break;
            }
        }   
    }

    //Event Listener
    private void CheckAndEnable(object sender, EventArgs args){
        if(gameStepID == InhalerLogic.Instance.CurrentStep){
            collider.enabled = true;  
        }
    }

    private void Disable(){
        collider.enabled = false;
    }

    //True: finger swiping down. Can be overridden by child class
    //Default function handles swiping down
    protected virtual bool IsDragging(Touch touch){
        bool retVal = false;
        if (startTouchPos.y - touch.position.y > minSwipeDistance){
            retVal = true;;
        }
        return retVal;
    }
}
