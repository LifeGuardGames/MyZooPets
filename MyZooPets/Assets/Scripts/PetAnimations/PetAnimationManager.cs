using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PetAnimationManager : Singleton<PetAnimationManager> {
	public Animator animator;

	public List<string> sadIdleAnimations;
	public List<string> happyIdleAnimations;
	public List<string> sick1IdleAnimations;
	public List<string> sick2IdleAnimations;
	public List<string> booleanParameters; //these parameters could be interrupted by isIdle so all of them need to be turned to false when isIdle is false

	private float timer = 0;
	private float timeBeforeNextRandomAnimation = 10f;
	private PetAnimStates currentAnimationState;

	public bool IsBusy{
		get{
			return currentAnimationState != PetAnimStates.Idling;
		}
	}

	void Start(){
		int mood = StatsController.Instance.GetStat(HUDElementType.Mood);
		int health = StatsController.Instance.GetStat(HUDElementType.Health);
		currentAnimationState = PetAnimStates.Idling;

		PetStatsModified(health, mood);
	}

	// Update is called once per frame
	void Update () {
		if(animator.IsInTransition(0)){ // reset timer if in transition
			timer = 0;
		}

		timer += Time.deltaTime;
		if(timer > timeBeforeNextRandomAnimation){
			timer = 0;

			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			List<string> animations = new List<string>();
			bool isIdle = animator.GetBool("IsIdle");

			if(isIdle){
				if(stateInfo.IsName("Base.SadIdle")){
					animations = sadIdleAnimations;
				}
				else if(stateInfo.IsName("Base.HappyIdle")){
					animations = happyIdleAnimations;
				}
				else if(stateInfo.IsName("Base.Sick1Idle")){
					animations = sick1IdleAnimations;
				}
				else if(stateInfo.IsName("Base.Sick2Idle")){
					animations = sick2IdleAnimations;
				}
				else{
					
				}
			}

			if(animations.Count != 0){
				int randomIndex = UnityEngine.Random.Range(0, animations.Count);
				animator.SetTrigger(animations[randomIndex]);
			}
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(0, 0, 100, 50), "Healthy")){
//			animator.SetInteger("Health", 80);
//		}
//
//		if(GUI.Button(new Rect(100, 0, 100, 50), "Sick1")){
//			animator.SetInteger("Health", 50);
//		}
//
//		if(GUI.Button(new Rect(200, 0, 100, 50), "Sick2")){
//			animator.SetInteger("Health", 20);
//		}
//
//		if(GUI.Button(new Rect(0, 50, 100, 50), "Happy")){
//			animator.SetInteger("Mood", 80);
//		}
//
//		if(GUI.Button(new Rect(100, 50, 100, 50), "Sad")){
//			animator.SetInteger("Mood", 30);
//		}
//	}

	public void PetStatsModified(int health, int mood){
		animator.SetInteger("Mood", mood);
		animator.SetInteger("Health", health);
	}

	public void StartWalking(){
		//turn all boolean parameter off before pet start walking animation
		foreach(string animationParameter in booleanParameters){
			animator.SetBool(animationParameter, false);
		}

		currentAnimationState = PetAnimStates.Walking;
		animator.SetBool("IsIdle", false);
	}

	public void StopWalking(){
		animator.SetBool("IsIdle", true);
		currentAnimationState = PetAnimStates.Idling;
	}

	/// <summary>
	/// Begins the fire blow. The breath in animation. Will be clamped forever on the last frame
	/// </summary>
	public void BeginFireBlow(){
		currentAnimationState = PetAnimStates.BreathingFire;
		animator.SetBool("IsFireBlowIn", true);
	}

	/// <summary>
	/// Abort fire blow will return back to idle animation
	/// </summary>
	public void AbortFireBlow(){
		animator.SetBool("IsFireBlowIn", false);
		currentAnimationState = PetAnimStates.Idling;
	}

	/// <summary>
	/// Finishes the fire blow. Blow out animation
	/// </summary>
	public void FinishFireBlow(){
		animator.SetTrigger("FireBlowOut");
		animator.SetBool("IsFireBlowIn", false);

		//need a call back from animation to know when to change the anim state
	}

	public void StartRubbing(){
		animator.SetBool("IsRubbing", true);
	}

	public void StopRubbing(){
		animator.SetBool("IsRubbing", false);
	}

	/// <summary>
	/// Waitings to be fed.
	/// </summary>
	public void WaitingToBeFed(){
		animator.SetBool("IsWaitingToEat", true);
	}

	/// <summary>
	/// Aborts the feeding.
	/// </summary>
	public void AbortFeeding(){
		animator.SetBool("IsWaitingToEat", false);
	}

	/// <summary>
	/// Feed the pet. Play the chew animation then return to the appropriate idle state
	/// </summary>
	public void Feed(){
		animator.SetTrigger("EatChew");
		animator.SetBool("IsWaitingToEat", false);
	}

	/// <summary>
	/// Starts the tickling. Tickle will be played until StopTickling() is called
	/// </summary>
	public void StartTickling(){
		animator.SetBool("IsTickling", true);
	}

	/// <summary>
	/// Stops the tickling.
	/// </summary>
	public void StopTickling(){
		animator.SetBool("IsTickling", false);

	}

	/// <summary>
	/// Flip the animation to the right if true
	/// </summary>
	/// <param name="isFlipped">If set to <c>true</c> is flipped.</param>
	public void Flip(bool isFlipped){
		if(isFlipped)
			animator.transform.localScale = new Vector3(-1, 1, 1);
		else{
			animator.transform.localScale = new Vector3(1, 1, 1);
		}
	}
}
