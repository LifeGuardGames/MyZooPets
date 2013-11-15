using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ButtonExitInhalerGame : ButtonChangeScene{

    //---------------------------------------------------
    // ProcessClick()
    //---------------------------------------------------   
    protected override void ProcessClick() {
        InhalerGameUIManager.Instance.CompleteTutorial();
        
        base.ProcessClick(); 
    }
}
