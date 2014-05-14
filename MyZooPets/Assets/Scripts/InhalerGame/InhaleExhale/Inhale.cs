using UnityEngine;
using System;
using System.Collections;

/*
    Handles inhale (swipe up) action
*/
public class Inhale : InhalerPart {
    public InhalerAnimationController animationController;

    protected override void Awake(){
        base.Awake();
        gameStepID = 7;
        floatyOptions.Add("text", ""); 
    }

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 

       if(direction == FingerGestures.SwipeDirection.Up){

            if(InhalerAnimationController.OnAnimDone == null){
                //Attach handler. so game can move on to next step after animation is done
                InhalerAnimationController.OnAnimDone += OnAnimationDone;

                //Disable hint when swipe gesture is registered. 
                GetComponent<HintController>().DisableHint(false);

                animationController.Inhale();
                AudioManager.Instance.PlayClip( "inhalerInhale" );      
            }
       }
    }

    protected override void Disable(){
        gameObject.SetActive(false);
    }

    protected override void Enable(){
        gameObject.SetActive(true);
    }

    protected override void NextStep(){
        base.NextStep();
        InhalerAnimationController.OnAnimDone -= OnAnimationDone;
        Destroy(gameObject);
    }

    private void OnAnimationDone(object sender, EventArgs args){
        NextStep(); 
    }
}
