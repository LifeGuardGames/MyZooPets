using UnityEngine;
using System.Collections;

/// <summary>
/// Animation control.
/// State machine wrapper for playing default animations (must be attached)
/// </summary>

public class AnimationControl : MonoBehaviour {
	public Animation thisAnimation;
	public bool resetAfterStop = false;
	private Vector3 originalPostion;
	private Quaternion originalRotation;
	private Vector3 originalScale;
	public GameObject optionalToggle;	// for use on SunBeamRotating and things like that
	public ParticleSystem optionalParticle;

	public bool debug = false;
	
	private bool isPlay = false;
	public bool isLooping;
	
	void Awake(){
		// Remember the original position if we need to reset it after playing, (Must be in awake, something killing it)
		originalPostion = gameObject.transform.localPosition;
		originalRotation = gameObject.transform.localRotation;
		originalScale = gameObject.transform.localScale;
	}

	void Start(){
		if(optionalToggle){
			optionalToggle.SetActive(false);
		}

		if(thisAnimation == null){
			thisAnimation = GetComponent<Animation>();
		}
	}

	void Update(){
		if(isLooping && isPlay && !thisAnimation.isPlaying){
			thisAnimation.Play();
		}
	}
	
	public void Play(string animationName){
		isPlay = true;
		thisAnimation.wrapMode = isLooping ? WrapMode.Loop : WrapMode.Once;
		thisAnimation.Play(animationName);

		if(optionalToggle){
			optionalToggle.SetActive(true);
		}
	}
	
	public void Play(bool isPlayParticle){
		isPlay = true;
		thisAnimation.wrapMode = isLooping ? WrapMode.Loop : WrapMode.Once;
		thisAnimation.Play();

		if(optionalToggle){
			optionalToggle.SetActive(true);
		}

		if(optionalParticle && isPlayParticle){
			optionalParticle.Play();
		}
	}

	public void Play(){
		isPlay = true;
		thisAnimation.wrapMode = isLooping ? WrapMode.Loop : WrapMode.Once;
		thisAnimation.Play();
		
		if(optionalToggle){
			optionalToggle.SetActive(true);
		}
		
		if(optionalParticle){
			optionalParticle.Play();
		}
	}

	public bool IsPlaying(string animName){
		return thisAnimation.IsPlaying(animName);
	}

	public void Stop(){
		isPlay = false;
		thisAnimation.Stop();

		if(optionalToggle){
			optionalToggle.SetActive(false);
		}

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
//					//Play("smallBounceHard");
//				StopAndResetFrame("zero");
//			}
//		}
//	}
}
