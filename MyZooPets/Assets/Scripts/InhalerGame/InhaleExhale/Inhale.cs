using UnityEngine;
using System;
using System.Collections;

/*
    Handles inhale (swipe up) action
*/
public class Inhale : SwipeToInhaleExhale {
    protected override void Start(){
        base.Start();
        InhalerLogic.OnResetGame += UpdateStepID;
    }    

    protected override void OnDestroy(){
        base.OnDestroy();
        InhalerLogic.OnResetGame -= UpdateStepID;
    }

    //True: finger swiping up
    protected override bool IsDragging(Touch touch){
        bool retVal = false;
        if (touch.position.y - startTouchPos.y > minSwipeDistance){
            retVal = true;
        }
        return retVal;
    } 

    private void UpdateStepID(object sender, EventArgs args){
        if(InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair){
            gameStepID = 5;
        }else if(InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            gameStepID = 6;
        }
    }

}
