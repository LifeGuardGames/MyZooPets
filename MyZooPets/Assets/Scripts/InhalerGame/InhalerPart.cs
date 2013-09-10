using UnityEngine;
using System;
using System.Collections;

/*
    This class sets up the basic structure of all the inhaler steps
    It listens to OnNextStep from InhalerLogic. Inhaler steps will be activated
    or deactivated at the appropriate steps
*/
public class InhalerPart : MonoBehaviour {
    protected int gameStepID; //The step in which the part will be activated

    protected virtual void Awake(){}

    protected virtual void Start(){
        InhalerLogic.OnNextStep += CheckAndEnable;
        Disable();
    }

    void OnDestroy(){
        InhalerLogic.OnNextStep -= CheckAndEnable; 
    }

    //Check to see if inhaler part can be activated
    private void CheckAndEnable(object sender, EventArgs args){
        if(InhalerLogic.Instance.CurrentStep == gameStepID)
            Enable();
    }

    //Hide Inhaler Part
    protected virtual void Disable(){}

    //Enable inhaler game part. 
    protected virtual void Enable(){}

    //Move to the next step of the inhaler game
    protected virtual void NextStep(){
        InhalerLogic.Instance.NextStep();
    }
}