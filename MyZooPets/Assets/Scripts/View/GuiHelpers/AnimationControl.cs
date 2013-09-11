using UnityEngine;
using System.Collections;

/// <summary>
/// Animation control.
/// State machine wrapper for playing default animations (must be attached)
/// </summary>

public class AnimationControl : MonoBehaviour {
	
	private bool isPlay = false;
	public bool isLooping;

	void Update(){
		if(isLooping && isPlay && !animation.isPlaying){
			animation.Play();
		}
	}
	
	public void Play(){
		isPlay = true;
		animation.wrapMode = isLooping ? WrapMode.Loop : WrapMode.Once;
		print (animation.wrapMode);
		animation.Play();
	}
		
	public void Stop(){
		isPlay = false;
	}
}
