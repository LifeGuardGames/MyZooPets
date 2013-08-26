using UnityEngine;
using System;
using System.Collections;

/*
    This generic class controls the hint arrows and message for inhaler parts
*/
public class HintController : MonoBehaviour {

    // To limit this hint arrow to show only for a specific type of inhaler,
    // set inhalerType to either "advair" or "rescue".
    public InhalerType inhalerType; 
    public bool startHidden = true; //Hide hint when game loads

    // Set this to the step that the hint arrow should be shown on.
    public int showOnStep = 0;
    public GameObject hintArrow; //Game object that contains the hint animation
    public GameObject hintMessage; //Game object that contains the hint message
    private tk2dSpriteAnimator arrowAnimator; //Sprite animator
	private Animation messageAnimation;
	private Animation arrowAnimation;

    void Awake(){
        if(hintArrow != null)
            arrowAnimator = hintArrow.GetComponent<tk2dSpriteAnimator>();
		if(hintMessage != null)
			messageAnimation = hintMessage.GetComponent<Animation>();
		if(hintArrow != null)
			arrowAnimation = hintArrow.GetComponent<Animation>();
    }

    void Start(){
        if(startHidden) DisableHint();
        else if(InhalerType.Advair == InhalerLogic.Instance.CurrentInhalerType && 
            !InhalerLogic.Instance.IsFirstTimeAdvair) DisableHint();
        else if(InhalerType.Rescue == InhalerLogic.Instance.CurrentInhalerType &&
            !InhalerLogic.Instance.IsFirstTimeRescue) DisableHint();

        InhalerLogic.OnNextStep += CheckAndEnableHint;
        InhalerGameManager.OnShowHint += CheckAndEnableHint;
    }

    void OnDestroy(){
        InhalerLogic.OnNextStep -= CheckAndEnableHint;
        InhalerGameManager.OnShowHint -= CheckAndEnableHint;
    }

    //Even Listener. Check if hint for the next step is necessary and disable hint for
    //current step 
    private void CheckAndEnableHint(object sender, EventArgs args){
        if(showOnStep == InhalerLogic.Instance.CurrentStep && 
            inhalerType == InhalerLogic.Instance.CurrentInhalerType &&
            InhalerGameManager.Instance.ShowHint){
            EnableHint();            
        }else{
            if((hintArrow != null && hintArrow.renderer.enabled) ||
                (hintMessage != null && hintMessage.renderer.enabled)){
                DisableHint();
            }
        }
    }

    //Turn off hint arrow and message
    private void DisableHint(){
        if(hintArrow != null) hintArrow.renderer.enabled = false;
        if(hintMessage != null) hintMessage.renderer.enabled = false;
        if(arrowAnimator != null) arrowAnimator.StopAndResetFrame();
		if(arrowAnimation != null) arrowAnimation.Stop();
		if(messageAnimation != null) messageAnimation.Stop();
    }

    //Turn on hint arrow and message
    private void EnableHint(){
        if(hintArrow != null) hintArrow.renderer.enabled = true;
        if(hintMessage != null) hintMessage.renderer.enabled = true;
        if(arrowAnimator != null) arrowAnimator.Play();
		if(arrowAnimation != null) arrowAnimation.Play();
		if(messageAnimation != null) messageAnimation.Play();
    }
}
