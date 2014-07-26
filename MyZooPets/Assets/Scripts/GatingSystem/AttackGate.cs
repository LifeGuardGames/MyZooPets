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
		
		// kick off attack animation
		this.attacker = attacker;
	}

	public void FinishAttack(){
		StartCoroutine(attacker.FinishFire());	
	}
	
	public void Cancel(){
		attacker.CancelFire();
		
		Destroy(this);
	}

	void OnDestroy(){
		// stop listening
		PetAnimator.OnAnimDone -= DoneAnimating;	
	}

	private void Attack(object sender, EventArgs args){ 
		attacker.BreathFire();
		FireMeter.OnFireReady -= Attack;
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
		
		// move the player because the gate just got pushed back (if it still exists)
//		if(gateTarget != null && !isDestroyed){
//			Vector3 vNewLoc = gateTarget.GetPlayerPosition();
//			PetMovement.Instance.MovePet(vNewLoc);
//		}
		
		// then we're done -- destroy ourselves
		Destroy(this);		
	}
}
