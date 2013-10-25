using UnityEngine;
using System.Collections;

///////////////////////////////////////////
// DegradParticle
// This is an individual particle that is
// spawned by an emitter that floats to
// the pet.
///////////////////////////////////////////	

public class DegradParticle : MonoBehaviour {
	// amount of damage this particle does
	private int nDamage;
	public int GetDamage() {
		return nDamage;
	}
	
	///////////////////////////////////////////
	// SetDamage()
	// Cache the damage this particle will do.
	///////////////////////////////////////////		
	public void SetDamage( int nDamage ) {
		this.nDamage = nDamage;
	}
	
	///////////////////////////////////////////
	// ReachedTarget()
	// Callback for when this particle hits
	// the target.
	///////////////////////////////////////////		
	private void ReachedTarget() {
		// deal damage to the pet's health
		int nDamage = GetDamage();
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, -nDamage, Vector3.zero, 0, Vector3.zero);
	}	
}
