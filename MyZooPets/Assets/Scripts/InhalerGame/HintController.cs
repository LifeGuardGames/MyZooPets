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
	public GameObject outlineTexture;	// Texture of outline if applicable
	public GameObject textObject;	// NGUI label of the text
	public GameObject customPlayFinger;	// Used for pinch, just enable the gameobject
	
	private Animation fingerAnimation;

    void Awake(){
		if(finger != null)
			fingerAnimation = finger.GetComponent<Animation>();

        if(startHidden){
            if(finger != null)
				finger.renderer.enabled = false;
			
			if(outlineTexture != null)
				outlineTexture.SetActive(false);
			
			if(textObject != null)
				textObject.SetActive(false);
			
			if(customPlayFinger != null)
				customPlayFinger.SetActive(false);
		}
			
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
    public void DisableHint(){
		if(fingerAnimation != null){
			if(fingerAnimation.IsPlaying(clipToPlay))
	            fingerAnimation.Stop(clipToPlay);
			
        //If no animation is currently playing turn the renderer off so no hints will be in the game
        if(!fingerAnimation.isPlaying)
            finger.renderer.enabled = false;
		}
		
		if(outlineTexture != null)
				outlineTexture.SetActive(false);
		
		if(textObject != null)
				textObject.SetActive(false);
		
		if(customPlayFinger != null)
				customPlayFinger.SetActive(false);
    }

    //Turn on hint
    private void EnableHint(){
		if(finger != null){
	        finger.renderer.enabled = true;
			fingerAnimation.Play(clipToPlay);
		}

		if(outlineTexture != null)
				outlineTexture.SetActive(true);
		
		if(textObject != null)
				textObject.SetActive(true);
		
		if(customPlayFinger != null)
				customPlayFinger.SetActive(true);
    }
}
