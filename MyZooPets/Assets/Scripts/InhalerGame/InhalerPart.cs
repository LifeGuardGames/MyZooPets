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
    protected string floatyText = "INHALER_FLOATY_NICE";

    //Initialize any protected variables here
    protected virtual void Awake(){}

    //Set up basic event handler. and disable inhaler part
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

    //Hide Inhaler Part. don't override this function if you don't want the inhaler part
    //to be disabled on start
    protected virtual void Disable(){}

    //Enable inhaler game part. 
    protected virtual void Enable(){}

    //Move to the next step of the inhaler game
    protected virtual void NextStep(){
        // spawn floaty text
        Hashtable option = new Hashtable();
        option.Add("parent", GameObject.Find("Anchor-Center"));
        option.Add("text", Localization.Localize(floatyText)); 
        option.Add("textSize", 128);
        FloatyUtil.SpawnFloatyText(option);

        InhalerLogic.Instance.NextStep();
    }
}