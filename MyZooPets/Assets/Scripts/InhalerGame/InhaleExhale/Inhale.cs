using UnityEngine;
using System;
using System.Collections;

/*
    Handles inhale (swipe up) action
*/
public class Inhale : InhalerPart {
    protected override void Awake(){
        gameStepID = 7;
    }

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 

       if(direction == FingerGestures.SwipeDirection.Up){
            NextStep();
       }
    }

    protected override void Disable(){
        gameObject.SetActive(false);
    }

    protected override void Enable(){
        gameObject.SetActive(true);
    }

    protected override void NextStep(){
		// play sound here
		AudioManager.Instance.PlayClip( "inhalerInhale" );		
		
        base.NextStep();
        Destroy(gameObject);
    }
}
