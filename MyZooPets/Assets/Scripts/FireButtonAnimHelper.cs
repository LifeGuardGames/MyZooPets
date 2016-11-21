using UnityEngine;
using UnityEngine.UI;

public class FireButtonAnimHelper : MonoBehaviour {
	public Animation buttonAnimation;
	public Animation enableFireButtonAnimation;
	public ParticleSystem buttonChargeParticle;
	public ParticleSystem buttonBurstParticle;

	public Image imageButton;
	public FireButtonHelper fireButtonHelper;
	public Sprite activeFireButtonSprite;
	public Sprite inactiveFireButtonSprite;

	public GameObject sunBeam;

	// Start the animation for the fire button enabling process, this will call the below 4 functions
	public void StartFireButtonAnimation(){
		enableFireButtonAnimation.Play("FireButtonEnableAnimation");
    }
	
	// This is called from the animation event
	public void StartChargeParticle(){
		buttonChargeParticle.Play();
	}
	
	// This is called from the animation event
	public void StartBurstParticle(){
		if(buttonBurstParticle){
			buttonBurstParticle.Play();
		}
	}

	// This is called from animation event, routes out and back in again to FireEffectOn
	public void TurnFireEffectOnAnimEvent() {
		FireButtonManager.Instance.TurnFireEffectOn(true);	// Need to use event handler
    }

	// This is called from FireButtonUIManager
	public void FireEffectOn(bool isAnimEvent){
		if(!isAnimEvent) {
			enableFireButtonAnimation.Stop();
		}
		buttonAnimation.Play();
		imageButton.sprite = activeFireButtonSprite;    //change button image 
		fireButtonHelper.Toggle3DCollider(false);
        sunBeam.SetActive(true);
    }

	public void FireEffectOff() {
		buttonAnimation.Stop();
		imageButton.sprite = inactiveFireButtonSprite;
		fireButtonHelper.Toggle3DCollider(true);
		sunBeam.SetActive(false);
	}

	// Audio handlers for animation event
	public void PlayAudioCharge(){
		AudioManager.Instance.PlayClip("fireCharge");
	}
	
	public void PlayAudioPop(){
		AudioManager.Instance.PlayClip("fireButtonPop");
	}
}
