using UnityEngine;
using System;
using System.Collections;

/*
    This class sets up the basic structure of all the inhaler steps
    It listens to OnNextStep from InhalerLogic. Inhaler steps will be activated
    or deactivated at the appropriate steps
*/
public class InhalerPart : MonoBehaviour {
	public Animator petAnimator;
    protected int gameStepID; //The step in which the part will be activated
    protected Hashtable floatyOptions;
	protected bool isGestureRecognized = false;

    //Initialize any protected variables here
    protected virtual void Awake(){
        floatyOptions = new Hashtable();    
    }

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
		if(InhalerLogic.Instance.CurrentStep == gameStepID) {
			Enable();
		}
    }

    //Hide Inhaler Part. don't override this function if you don't want the inhaler part
    //to be disabled on start
    protected virtual void Disable(){}

    //Enable inhaler game part. 
    protected virtual void Enable(){}

    //Move to the next step of the inhaler game
    protected virtual void NextStep(){
        // spawn floaty text
//        floatyOptions.Add("parent", GameObject.Find("Anchor-Center"));
//		floatyOptions.Add("prefab", "FloatyTextInhalerGame");
//        if(!floatyOptions.ContainsKey("text"))
//            floatyOptions.Add("text", Localization.Localize("INHALER_FLOATY_NICE")); 
//        if(!floatyOptions.ContainsKey("textSize"))
//            floatyOptions.Add("textSize", 128f);
//        FloatyUtil.SpawnFloatyText(floatyOptions);

		InhalerLogic.Instance.NextStep();
    }

}