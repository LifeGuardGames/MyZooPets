using UnityEngine;

/// <summary>
/// Used for event callbacks from unity animations
/// </summary>
public class AnimationSoundController : MonoBehaviour {
	public void PlayClip(string clipName){
		AudioManager.Instance.PlayClip(clipName);
	}
}
