using UnityEngine;
using System.Collections;

/// <summary>
/// Animation control.
/// State machine wrapper for playing default animations (must be attached)
/// </summary>

public class AnimationControl : MonoBehaviour {
	
	private bool isPlay = false;
	public bool isLooping;
	
	void OnGUI(){
		if(GUI.Button(new Rect(20, 20, 100, 20), "start")){
			Play();
		}
		if(GUI.Button(new Rect(20, 50, 1000, 20), "stop")){
			Stop();
		}
	}
	
	void Update(){
		if(isLooping && isPlay && !animation.isPlaying){
			animation.Play();
		}
	}
	
	public void Play(){
		isPlay = true;
		animation.Play();
	}
		
	public void Stop(){
		isPlay = false;
	}
}
