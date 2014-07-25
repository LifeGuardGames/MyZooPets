using UnityEngine;
using System.Collections;

/// <summary>
/// Handle case for audio in animation events
/// </summary>
public class AnimatioEventAudio : MonoBehaviour {
	public string sound1;
	public string sound2;
	public string sound3;

	public void PlaySound1(){
		AudioManager.Instance.PlayClip(sound1);
	}

	public void PlaySound2(){
		AudioManager.Instance.PlayClip(sound2);
	}

	public void PlaySound3(){
		AudioManager.Instance.PlayClip(sound3);
	}
}
