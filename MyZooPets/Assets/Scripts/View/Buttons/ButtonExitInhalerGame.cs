using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ButtonExitInhalerGame : ButtonChangeScene{

    //---------------------------------------------------
    // ProcessClick()
    //---------------------------------------------------   
    protected override void ProcessClick() {
        InhalerLogic.Instance.CompleteTutorial();

        Analytics.Instance.InhalerSwipeSequences(Analytics.STEP_STATUS_QUIT, InhalerLogic.Instance.CurrentStep);
        Analytics.Instance.EndPlayTimeTracker();

        base.ProcessClick(); 
    }
}
