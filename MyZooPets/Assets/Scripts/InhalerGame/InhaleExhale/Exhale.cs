using UnityEngine;
using System;
using System.Collections;

/*
    Handles exhale (Step 4)
    Listens to swipe gesture. 
*/
public class Exhale : InhalerPart {
    protected override void Awake(){
        gameStepID = 4;
    }

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 

       if(direction == FingerGestures.SwipeDirection.Down){
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
        base.NextStep();
        Destroy(gameObject);
    }
}
