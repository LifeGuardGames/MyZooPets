using UnityEngine;
using System.Collections;

/// <summary>
/// Animation control.
/// State machine wrapper for playing default animations (must be attached)
/// </summary>

public class AnimationControl : MonoBehaviour {
	
	public bool debug = false;
	
	private bool isPlay = false;
	public bool isLooping;

	void Update(){
		if(isLooping && isPlay && !animation.isPlaying){
			animation.Play();
		}
	}
	
	public void Play(string animationName){
		isPlay = true;
		animation.Play(animationName);
	}
	
	public void Play(){
		isPlay = true;
		animation.wrapMode = isLooping ? WrapMode.Loop : WrapMode.Once;
		animation.Play();
	}
		
	public void Stop(){
		isPlay = false;
		animation.Stop();
	}
	
	// Rewinds the frame to original position
	public void StopAndResetFrame(string resetName){
		Play(resetName);
		StartCoroutine(stopNextFrame());
	}
	
	private IEnumerator stopNextFrame(){
		yield return 0;
		Stop();
	}
	
//	void OnGUI(){
//		if(debug){
//			if(GUI.Button(new Rect(100, 100, 100, 100), "start")){
//				Play();
//			}
//			if(GUI.Button(new Rect(200, 100, 100, 100), "Stop + Reset")){
//				//Play("smallBounceSoftWellapad");
//				StopAndResetFrame("zero");
//			}
//		}
//	}
}
