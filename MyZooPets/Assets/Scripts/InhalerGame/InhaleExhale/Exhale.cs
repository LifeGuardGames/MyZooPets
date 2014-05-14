﻿using UnityEngine;
using System;
using System.Collections;

/*
    Handles exhale (Step 4)
    Listens to swipe gesture. 
*/
public class Exhale : InhalerPart {
    public InhalerAnimationController animationController;

    protected override void Awake(){
        base.Awake();
        gameStepID = 4;
    }

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 

       if(direction == FingerGestures.SwipeDirection.Down){

            //Only proceed with this step if no listener is registered to OnAnimDone
            //This is to fix the problem when the user swipes really fast during exhale
            //the same listener is registered to the same event multiple time, causing
            //skipping game steps
            if(InhalerAnimationController.OnAnimDone == null){
                //Attach handler. so game can move on to next step after animation is done
                InhalerAnimationController.OnAnimDone += OnAnimationDone;

                //Disable hint when swipe gesture is registered. 
                GetComponent<HintController>().DisableHint(false);

                animationController.Exhale();
                AudioManager.Instance.PlayClip( "inhalerExhale" );      
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
