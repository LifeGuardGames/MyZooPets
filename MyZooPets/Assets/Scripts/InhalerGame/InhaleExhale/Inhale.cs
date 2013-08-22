using UnityEngine;
using System.Collections;

/*
    Handles inhale (swipe up) action
*/
public class Inhale : SwipeToInhaleExhale {
    protected override void Start(){
        base.Start();
        gameStepID = 6;
    }    
    //True: finger swiping up
    protected override bool IsDragging(Touch touch){
        bool retVal = false;
        if (touch.position.y - startTouchPos.y > minSwipeDistance){
            retVal = true;
        }
        return retVal;
    } 

}
