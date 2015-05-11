using UnityEngine;
using System.Collections;

// DegradParticle
// This is an individual particle that is spawned by an emitter that floats to the pet.
public class DegradParticle : MonoBehaviour{
	private int damage;				// amount of damage this particle does
	public int Damage{
		get{ return damage; }
		set{ damage = value; }
	}

	public ParticleSystem particleBlast;
	public ParticleSystem particleStream;

	// ReachedTarget()
	// Callback for when this particle hits the target.	
	private void ReachedTarget(){
		DegradationLogic.Instance.TriggerHitPet(this);
		particleBlast.Play();
		particleStream.Stop();
	}	
}
