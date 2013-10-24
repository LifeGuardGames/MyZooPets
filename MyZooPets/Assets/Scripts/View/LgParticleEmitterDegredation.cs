using UnityEngine;
using System.Collections;

public class LgParticleEmitterDegredation : LgParticleEmitter {
	
	public GameObject targetDestination;
	
	// amount of damage the emitter particles cause the pet's health when they collide
	private int nDamage;
	private int GetDamage() {
		return nDamage;	
	}
	
	protected override void _ExtendedAction(GameObject emittedObject){
		MoveTowards moveScript = emittedObject.GetComponent<MoveTowards>();
		if(moveScript != null){
			moveScript.Target = targetDestination.transform;
			moveScript.touchCallbackTarget = gameObject;
		}
		else{
			Debug.LogError("No MoveTowards script detected in particle");
		}
	}
	
	///////////////////////////////////////////
	// InitDegradEmitter()
	// Sets some basic values for the emitter
	// and what kind of damage it causes.
	///////////////////////////////////////////		
	public void InitDegradEmitter( float fTime, int nDamage ) {
		minInterval = fTime;
		maxInterval = fTime;
		this.nDamage = nDamage;
	}
	
	///////////////////////////////////////////
	// ReachedTarget()
	// Callback for when particles emitted by
	// this emitter hit their target.
	///////////////////////////////////////////		
	private void ReachedTarget() {
		// deal damage to the pet's health
		int nDamage = GetDamage();
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, -nDamage, Vector3.zero, 0, Vector3.zero);
	}
}
