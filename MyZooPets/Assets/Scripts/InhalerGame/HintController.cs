using UnityEngine;
using System;
using System.Collections;

/*
    This generic class controls the hint arrows and message for inhaler parts
*/
public class HintController : MonoBehaviour {
    public bool startHidden = true; //Hide hint when game loads
    public int showOnStep = 0; // Set this to the step that the hint arrow should be shown on.
    public GameObject finger; //Game object that contains the hint animation
	public string clipToPlay; //The animation that you want to play
	
	private Animation fingerAnimation;

    void Awake(){
		if(finger != null)
			fingerAnimation = finger.GetComponent<Animation>();

        if(startHidden)
            finger.renderer.enabled = false;

        InhalerGameUIManager.OnShowHint += CheckAndEnableHint;
    }

    void OnDestroy(){
        InhalerGameUIManager.OnShowHint -= CheckAndEnableHint;
    }

    //Event Listener. Check if hint for the next step is necessary and disable hint for
    //current step 
    private void CheckAndEnableHint(object sender, EventArgs args){
        if(showOnStep == InhalerLogic.Instance.CurrentStep && 
            InhalerGameUIManager.Instance.ShowHint){
            EnableHint();            
        }else{
            DisableHint();
        }
    }

    //Turn off hint 
    private void DisableHint(){
        if(fingerAnimation.IsPlaying(clipToPlay))
            fingerAnimation.Stop(clipToPlay);

        //If no animation is currently playing turn the renderer off so no hints will be in the game
        if(!fingerAnimation.isPlaying)
            finger.renderer.enabled = false;
    }

    //Turn on hint
    private void EnableHint(){
        finger.renderer.enabled = true;
		fingerAnimation.Play(clipToPlay);
    }
}
