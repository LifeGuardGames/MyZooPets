using UnityEngine;
using System;
using System.Collections;

/*
    This generic class controls the hint arrows and message for inhaler parts
*/
public class HintController : MonoBehaviour {
    public bool startHidden = true; //Hide hint when game loads
    public int showOnStep = 0; // Set this to the step that the hint arrow should be shown on.
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

        InhalerLogic.OnNextStep += CheckAndEnableHint;
        InhalerGameUIManager.OnShowHint += CheckAndEnableHint;
    }

    void OnDestroy(){
        InhalerLogic.OnNextStep -= CheckAndEnableHint;
        InhalerGameUIManager.OnShowHint -= CheckAndEnableHint;
    }

    //Even Listener. Check if hint for the next step is necessary and disable hint for
    //current step 
    private void CheckAndEnableHint(object sender, EventArgs args){
        if(showOnStep == InhalerLogic.Instance.CurrentStep && 
            InhalerGameUIManager.Instance.ShowHint){
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
