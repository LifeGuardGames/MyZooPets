﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// AttackGate
// Script put on a pet when it is to attack a gate.
//---------------------------------------------------

public class AttackGate : MonoBehaviour {
	// gate to attack
	private Gate gateTarget;
	
	// damage to deal
	private int nDamage;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------
	public void Init( PetAnimator attacker, Gate gateTarget, int nDamage ) {
		this.gateTarget = gateTarget;
		this.nDamage = nDamage;
		
		// listen for anim complete message on pet
		PetAnimator.OnAnimDone += DoneAnimating;
		
		// kick off attack animation
		attacker.BreathFire();
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------	
	private void OnDestroy() {
		// stop listening
		PetAnimator.OnAnimDone -= DoneAnimating;	
	}
	
	//---------------------------------------------------
	// DoneAnimating()
	// For when the pet is done animating.
	//---------------------------------------------------	
	private void DoneAnimating( object sender, PetAnimArgs args ) {
		if ( args.GetAnimState() == PetAnimStates.BreathingFire ) 
			StartCoroutine(DoneAttacking());
	}
	
	//---------------------------------------------------
	// DoneAttacking()
	// For when the pet is done breathing fire.
	//---------------------------------------------------		
	private IEnumerator DoneAttacking() {
		// damage the gate
		gateTarget.DamageGate( nDamage );
		
		// wait a frame to do our other stuff because the fire breathing animation is still technically playing
		yield return 0;
		
		// move the player because the gate just got pushed back (if it still exists)
		if ( gateTarget != null ) {
			Vector3 vNewLoc = gateTarget.GetPlayerPosition();
			PetMovement.Instance.MovePet( vNewLoc );
		}
		
		// then we're done -- destroy ourselves
		Destroy( this );		
	}
}