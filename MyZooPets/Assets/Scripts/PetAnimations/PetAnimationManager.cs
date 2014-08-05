﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PetAnimationManager : Singleton<PetAnimationManager> {
	public Animator animator;
	public GameObject flippableComponents;
	public GameObject fireBlowPosition;
	public GameObject headCollider;
	public GameObject bodyCollider;
	public GameObject highFiveCollider;


	public List<string> sadIdleAnimations;
	public List<string> happyIdleAnimations;
	public List<string> sick1IdleAnimations;
	public List<string> sick2IdleAnimations;
	public List<string> booleanParameters; //these parameters could be interrupted by isIdle so all of them need to be turned to false when isIdle is false

	private float idleStateTimer = 0;
	private float timeBeforeNextRandomIdleAnimation = 10f;
	private float highFiveWaitTimer = 0;
	private float timeBeforeHighFiveEnds = 5f;
	private PetAnimStates currentAnimationState;

	private FireBlowParticleController fireScript;

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
		RunIdleStateTimer();
		RunHighFiveTimer();
	}


//	void OnGUI(){

//		if(GUI.Button(new Rect(0, 0, 100, 50), "Healthy")){
////			animator.SetInteger("Health", 80);
//			StartHighFive();
//		}

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

	/// <summary>
	/// Pet stats have been modified so make sure idle state animation reflects
	/// the current health and mood
	/// </summary>
	/// <param name="health">Health.</param>
	/// <param name="mood">Mood.</param>
	public void PetStatsModified(int health, int mood){
		animator.SetInteger("Mood", mood);
		animator.SetInteger("Health", health);
	}

	/// <summary>
	/// Starts walking animation. 
	/// Any state animation
	/// </summary>
	public void StartWalking(){
		CheckConditionBeforeAnyStateAnimation();

		//turn all boolean parameter off before pet start walking animation
		foreach(string animationParameter in booleanParameters){
			animator.SetBool(animationParameter, false);
		}

		currentAnimationState = PetAnimStates.Walking;
		animator.SetBool("IsIdle", false);
	}

	/// <summary>
	/// Stop walking animation
	/// </summary>
	public void StopWalking(){
		animator.SetBool("IsIdle", true);
		currentAnimationState = PetAnimStates.Idling;
	}

	/// <summary>
	/// Begins the fire blow. The breath in animation. Will be clamped forever on the last frame
	/// </summary>
	public void StartFireBlow(){
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

		// spawn the particle effect
		Skill curSkill = FlameLevelLogic.Instance.GetCurrentSkill();
		string flameResourceString = curSkill.FlameResource;
		GameObject flamePrefab = Resources.Load(flameResourceString) as GameObject;
		GameObject flameObject = Instantiate(flamePrefab, new Vector3(0, 0, 0), flamePrefab.transform.rotation) as GameObject;
		
		// parent it to the right position
		flameObject.transform.parent = fireBlowPosition.transform;				
		flameObject.transform.localPosition = new Vector3(0, 0, 0);
		
		// actually kick off the effect
		fireScript = flameObject.GetComponent<FireBlowParticleController>();
		fireScript.Play();
	}

	/// <summary>
	/// Called by the PetAnimationEventHandler when the FireBlowOut animation is
	/// complete
	/// </summary>
	public void DoneWithFireBlowAnimation(){
		currentAnimationState = PetAnimStates.Idling;

		fireScript.Stop();

		if(AttackGate.Instance)
			AttackGate.Instance.ExecutePostAttackLogic();
	}

	public void StartRubbing(){
		animator.SetBool("IsRubbing", true);
	}

	public void StopRubbing(){
		animator.SetBool("IsRubbing", false);
	}

	/// <summary>
	/// Waitings to be fed.
	/// Any state animation
	/// </summary>
	public void WaitingToBeFed(){
		CheckConditionBeforeAnyStateAnimation();

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
	public void FinishFeeding(){
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
	/// Starts high five animation.
	/// </summary>
	public void StartHighFive(){
		highFiveCollider.SetActive(true);
		headCollider.SetActive(false);
		bodyCollider.SetActive(false);
		animator.SetBool("IsHighFiving", true);
	}

	public void FinishHighFive(){
		highFiveCollider.SetActive(false);
		headCollider.SetActive(true);
		bodyCollider.SetActive(true);
		animator.SetBool("IsHighFiving", false);
		animator.SetTrigger("YesHighFive");
	}
	
	/// <summary>
	/// Flip the animation to the right if true
	/// </summary>
	/// <param name="isFlipped">If set to <c>true</c> is flipped.</param>
	public void Flip(bool isFlipped){
		if(isFlipped)
			flippableComponents.transform.localScale = new Vector3(-1, 1, 1);
		else{
			flippableComponents.transform.localScale = new Vector3(1, 1, 1);
		}
	}

	/// <summary>
	/// Any state animation
	/// </summary>
	public void Swat(){
		CheckConditionBeforeAnyStateAnimation();

		animator.SetTrigger("Swat");
	}

	/// <summary>
	/// Call this function before an any state animation is executed to do a
	/// check to see if there's anything that needs to be turned on or off before
	/// interrupting the current animation
	/// </summary>
	private void CheckConditionBeforeAnyStateAnimation(){
		bool isHighFiving = animator.GetBool("IsHighFiving");
		if(isHighFiving){
			animator.SetBool("IsHighFiving", false);
			highFiveCollider.SetActive(false);
			headCollider.SetActive(true);
			bodyCollider.SetActive(true);
		}
	}

	private void RunHighFiveTimer(){
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		
		//only run the timer if high 5 loop animation is currenlty playing
		if(stateInfo.IsName("Base.High5Loop")){
			highFiveWaitTimer += Time.deltaTime;
			if(highFiveWaitTimer > timeBeforeHighFiveEnds){
				highFiveWaitTimer = 0;
				
				//user didn't high five pet so end the high five animation
				highFiveCollider.SetActive(false);
				headCollider.SetActive(true);
				bodyCollider.SetActive(true);
				animator.SetBool("IsHighFiving", false);
				animator.SetTrigger("NoHighFive");
			}
		}
	}

	/// <summary>
	/// Runs the idle state timer. Play a random idle animation after counting down
	/// </summary>
	private void RunIdleStateTimer(){
		if(animator.IsInTransition(0)){ // reset timer if in transition
			idleStateTimer = 0;
		}
		
		idleStateTimer += Time.deltaTime;
		if(idleStateTimer > timeBeforeNextRandomIdleAnimation){
			idleStateTimer = 0;
			
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			List<string> animations = new List<string>();
			bool isIdle = animator.GetBool("IsIdle");

			//if idle figure out which idle state is the animator in
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

			//pick the appropriate random idle animation
			if(animations.Count != 0){
				int randomIndex = UnityEngine.Random.Range(0, animations.Count);
				animator.SetTrigger(animations[randomIndex]);
			}
		}
	}
}
