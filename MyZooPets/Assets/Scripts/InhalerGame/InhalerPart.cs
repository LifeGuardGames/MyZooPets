using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// This class sets up the basic structure of all the inhaler steps
/// It listens to OnNextStep from InhalerLogic.Inhaler steps will be activated
/// or deactivated at the appropriate steps
/// </summary>
public class InhalerPart : MonoBehaviour {
	public Animator petAnimator;
    protected int gameStepID;					//The step in which the part will be activated
	protected bool isGestureRecognized = false;

    //Set up basic event handler. and disable inhaler part
    protected virtual void Start(){
        InhalerGameManager.OnNextStep += CheckAndEnable;
        Disable();
    }

    void OnDestroy(){
        InhalerGameManager.OnNextStep -= CheckAndEnable; 
    }

    //Check to see if inhaler part can be activated
    private void CheckAndEnable(object sender, EventArgs args){
		if(InhalerGameManager.Instance.CurrentStep == gameStepID) {
			Enable();
		}
    }

    //Hide Inhaler Part. don't override this function if you don't want the inhaler part
    //to be disabled on start
    protected virtual void Disable(){}

    //Enable inhaler game part
    protected virtual void Enable(){}

    //Move to the next step of the inhaler game
    protected virtual void NextStep(){
		InhalerGameManager.Instance.NextStep();
    }
}