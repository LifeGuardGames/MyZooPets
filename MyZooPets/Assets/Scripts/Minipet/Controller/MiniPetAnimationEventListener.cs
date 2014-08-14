using UnityEngine;
using System.Collections;

public class MiniPetAnimationEventListener : MonoBehaviour {

	public void PlaySoundClip(string clipName){
		if(!string.IsNullOrEmpty(clipName)){
			Hashtable option = new Hashtable();
			option.Add("IsInterruptingRecurringClip", true);
			MiniPetAudioManager.Instance.PlayClip(clipName, option);
		}
	}

	public void PlayRecurringSoundClip(string clipName){
		if(!string.IsNullOrEmpty(clipName))
			MiniPetAudioManager.Instance.PlayRecurringClip(clipName);
	}

	public void PlayLoopingSoundClip(string clipName){
		if(!string.IsNullOrEmpty(clipName))
			MiniPetAudioManager.Instance.PlayLoopingClip(clipName);
	}
}
