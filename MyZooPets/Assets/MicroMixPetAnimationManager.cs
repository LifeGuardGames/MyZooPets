using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MicroMixPetAnimationManager : Singleton<MicroMixPetAnimationManager> {

	public static EventHandler<EventArgs> OnBreathEnded; //event sent out when fire blow animation ended

	public Animator animator;
	public GameObject animatorObject;
	public GameObject flippableComponents;
	public GameObject fireBlowPosition;

	public List<string> sadIdleAnimations;
	public List<string> happyIdleAnimations;
	public List<string> sick1IdleAnimations;
	public List<string> sick2IdleAnimations;
	public List<string> booleanParameters; //these parameters could be interrupted by isIdle so all of them need to be turned to false when isIdle is false

	private PetAnimStates currentAnimationState;
	private FireBlowParticleController fireScript;

	void Start() {
		currentAnimationState = PetAnimStates.Idling;
	}

	/// <summary>
	/// Begins the fire blow. The breath in animation. Will be clamped forever on the last frame
	/// </summary>
	public void StartFireBlow() {
		currentAnimationState = PetAnimStates.BreathingFire;
		animator.SetBool("IsFireBlowIn", true);
	}

	/// <summary>
	/// Abort fire blow will return back to idle animation
	/// </summary>
	public void AbortFireBlow() {
		animator.SetBool("IsFireBlowIn", false);
		currentAnimationState = PetAnimStates.Idling;
		PetAudioManager.Instance.StopAnimationSound();
	}

	/// <summary>
	/// Finishes the fire blow. Blow out animation
	/// </summary>
	public void FinishFireBlow() {
		animator.SetTrigger("FireBlowOut");
		animator.SetBool("IsFireBlowIn", false);

		// spawn the particle effect
		ImmutableDataSkill curSkill = DataLoaderSkills.GetFlameAtLevel((int)LevelLogic.Instance.CurrentLevel);
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
	public void DoneWithFireBlowAnimation() {
		currentAnimationState = PetAnimStates.Idling;

		fireScript.Stop();

		if(OnBreathEnded != null) {
			OnBreathEnded(this, EventArgs.Empty);
		}
	}

	public void Flipping() {
		animator.SetTrigger("Backflip");
	}
}
