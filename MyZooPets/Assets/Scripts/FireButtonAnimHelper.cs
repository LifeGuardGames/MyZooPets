using UnityEngine;

public class FireButtonAnimHelper : MonoBehaviour {
	public Animation buttonAnimation;
	public Animation enableFireButtonAnimation;
	public ParticleSystem buttonChargeParticle;
	public ParticleSystem buttonBurstParticle;

	// Start the animation for the fire button enabling process, this will call the below 4 functions
	public void StartFireButtonAnimation(){
		enableFireButtonAnimation.Play();
	}

	// This is called from the animation event
	public void FireButtonAnimationActivate(){
		enableFireButtonAnimation.Stop();
		buttonAnimation.Play();
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

	public void FireEffectOn(){
		FireButtonUIManager.Instance.FireEffectOn(1);
	}

	// Audio handlers for animation event
	public void PlayAudioCharge(){
		AudioManager.Instance.PlayClip("fireCharge");
	}
	
	public void PlayAudioPop(){
		AudioManager.Instance.PlayClip("fireButtonPop");
	}
}
