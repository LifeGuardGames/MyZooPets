using UnityEngine;
using System.Collections;

/// <summary>
/// Animation control.
/// State machine wrapper for playing default animations (must be attached)
/// </summary>

public class AnimationControl : MonoBehaviour {
	
	public bool resetAfterStop = false;
	private Vector3 originalPostion;
	private Quaternion originalRotation;
	private Vector3 originalScale;
	
	public bool debug = false;
	
	private bool isPlay = false;
	public bool isLooping;
	
	void Awake(){
		// Remember the original position if we need to reset it after playing, (Must be in awake, something killing it)
		originalPostion = gameObject.transform.localPosition;
		originalRotation = gameObject.transform.localRotation;
		originalScale = gameObject.transform.localScale;
	}
	
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

	public bool IsPlaying(string animName){
		return animation.IsPlaying(animName);
	}
		
	public void Stop(){
		isPlay = false;
		animation.Stop();
		
		if(resetAfterStop){
			gameObject.transform.localPosition = originalPostion;
			gameObject.transform.localRotation = originalRotation;
			gameObject.transform.localScale = originalScale;
		}
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
