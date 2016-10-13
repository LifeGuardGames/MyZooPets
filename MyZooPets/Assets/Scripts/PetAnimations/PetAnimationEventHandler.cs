using UnityEngine;
using System.Collections;

/// <summary>
/// Pet animation event listener. All the animation events fired by pet related
/// animations are handled here
/// </summary>
public class PetAnimationEventHandler : MonoBehaviour {
	public void PetAnimationEvent(string eventName){
		switch(eventName){
		case "FinishBlowingFire":
			if(PetAnimationManager.Instance != null) {
				PetAnimationManager.Instance.DoneWithFireBlowAnimation();
			}
			else {
				MicroMixPetAnimationManager.Instance.DoneWithFireBlowAnimation();
			}
			break;
		}
	}

	public void PlaySoundClip(string animationID){
		if(!string.IsNullOrEmpty(animationID)){
			PetAudioManager.Instance.PlayAnimationSound(animationID);
		}
	}

	public void PlayRecurringSoundClip(string animationID){
		if(!string.IsNullOrEmpty(animationID))
			PetAudioManager.Instance.PlayRecurringClip(animationID);
	}

	public void PlayLoopingSoundClip(string animationID){
		if(!string.IsNullOrEmpty(animationID))
			PetAudioManager.Instance.PlayLoopingClip(animationID);
	}
}
