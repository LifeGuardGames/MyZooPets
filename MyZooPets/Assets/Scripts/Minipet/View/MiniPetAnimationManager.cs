using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MiniPetAnimationManager : MonoBehaviour {

	public List<string> happyIdleAnimations;

	private Animator animator;
	private bool isRunningHappyIdleAnimations;
	private float happyIdleStateTimer = 0;
	private float timeBeforeNextRandomHappyIdleAnimation = 5f;

	public bool IsRunningIdleAnimations{
		get{ return isRunningHappyIdleAnimations;}
		set{ isRunningHappyIdleAnimations = value;}
	}

	void Awake(){
		isRunningHappyIdleAnimations = true;
		animator = this.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		if(isRunningHappyIdleAnimations)
			RunIdleStateTimer();
	}

	/// <summary>
	/// Play cheer animation.
	/// </summary>
	public void Cheer(){
		animator.SetTrigger("Cheer");
	}

	public void Eat(){
		animator.SetTrigger("Eat");
	}

	public bool IsTickling(){
		return animator.GetBool("IsTickling");
	}

	/// <summary>
	/// Start tickling animation.
	/// </summary>
	public void StartTickling(){
		animator.SetBool("IsTickling", true);
	}

	/// <summary>
	/// Stop tickling animation.
	/// </summary>
	public void StopTickling(){
		animator.SetBool("IsTickling", false);
		MiniPetAudioManager.Instance.StopLoopingClip();
	}

	/// <summary>
	/// Start sad animation and pause idle animation timer (no sad idle animation right now).
	/// </summary>
	public void Sad(){
		animator.SetBool("IsSad", true);
		isRunningHappyIdleAnimations = false;
	}

	public void NotSad(){
		animator.SetBool("IsSad", false);
		MiniPetAudioManager.Instance.StopRecurringClip();
	}

	/// <summary>
	/// Runs the idle state timer. Plays a random idle animation after count down timer
	/// </summary>
	private void RunIdleStateTimer(){
		if(animator.IsInTransition(0)){ // reset timer if in transition
			happyIdleStateTimer = 0;
		}
		
		happyIdleStateTimer += Time.deltaTime;
		if(happyIdleStateTimer > timeBeforeNextRandomHappyIdleAnimation){
			happyIdleStateTimer = 0;
			
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			List<string> animations = new List<string>();
			animations = happyIdleAnimations;

			//pick the appropriate random idle animation
			if(animations.Count != 0){
				int randomIndex = UnityEngine.Random.Range(0, animations.Count);
				animator.SetTrigger(animations[randomIndex]);
			}
		}
	}
}
