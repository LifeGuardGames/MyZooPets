using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attack gate. Script put on a pet when it is ready to attack a gate
/// </summary>
public class AttackGate : Singleton<AttackGate>{

	private Gate gateTarget; // gate to attack
	private int damage; // damage to deal
	private PetAnimator attacker; // the pet

	public void Init(PetAnimator attacker, Gate gateTarget, int damage){
		this.gateTarget = gateTarget;
		this.damage = damage;
		
		// listen for anim complete message on pet
		PetAnimator.OnAnimDone += DoneAnimating;
		FireMeter.OnFireReady += Attack;
		FireMeter.OnMeterFilled += DisableAttack;
		
		// kick off attack animation
		this.attacker = attacker;
	}

	void OnDestroy(){
		// stop listening
		PetAnimator.OnAnimDone -= DoneAnimating;
		FireMeter.OnFireReady -= Attack;
		FireMeter.OnMeterFilled -= DisableAttack;
	}

	public void FinishAttack(){
		StartCoroutine(attacker.FinishFire());	
	}

	/// <summary>
	/// Cancel attack so clean up.
	/// </summary>
	public void Cancel(){
		attacker.CancelFire();

		PetAnimator.OnAnimDone -= DoneAnimating;
		FireMeter.OnFireReady -= Attack;
		FireButtonUIManager.Instance.fireButtonCollider.enabled = true;

		//release lock if fire breathing lock was called previously
		UIModeTypes currentLockMode = ClickManager.Instance.CurrentMode;
		if(currentLockMode == UIModeTypes.FireBreathing)
			ClickManager.Instance.ReleaseLock();
		
		Destroy(this);
	}
	
	private void Attack(object sender, EventArgs args){ 
		attacker.BreathFire();
		FireMeter.OnFireReady -= Attack;

		ClickManager.Instance.Lock(mode:UIModeTypes.FireBreathing);
	}

	private void DisableAttack(object sender, EventArgs args){
		FireButtonUIManager.Instance.fireButtonCollider.enabled = false;
	}

	/// <summary>
	/// When pet is done animating
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void DoneAnimating(object sender, PetAnimArgs args){
		if(args.GetAnimState() == PetAnimStates.BreathingFire){
			StartCoroutine(DoneAttacking());
		}
	}
	
	/// <summary>
	/// Pet done attacking. 
	/// </summary> 
	private IEnumerator DoneAttacking(){
		// and decrement the user's fire breaths
		StatsController.Instance.ChangeFireBreaths(-1);

		// damage the gate
		bool isDestroyed = gateTarget.DamageGate(damage);

		// also mark the player as having attack the monster (for wellapad tasks)
		WellapadMissionController.Instance.TaskCompleted("FightMonster");
		
		// wait a frame to do our other stuff because the fire breathing animation is still technically playing
		yield return 0;

		//make button clickable again
		if(FireButtonUIManager.Instance)
			FireButtonUIManager.Instance.fireButtonCollider.enabled = true;

		// release fire breathing lock
		ClickManager.Instance.ReleaseLock();

		// then we're done -- destroy ourselves
		Destroy(this);		
	}
}
