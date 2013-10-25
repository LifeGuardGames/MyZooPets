using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LgParticleEmitterDegredation : LgParticleEmitter {
	// list of objects this emitter has spawned
	List<GameObject> listSpawned = new List<GameObject>();
	
	public GameObject targetDestination;
	
	// amount of damage the emitter particles cause the pet's health when they collide
	private int nDamage;
	private int GetDamage() {
		return nDamage;	
	}
	
	protected override void _Start() {
		// listen for when the player changes partitions, as this emitter may need to act
		CameraManager.Instance.GetPanScript().OnPartitionChanging += OnPartitionChanging;		
	}
	
    protected override void _OnDestroy(){
		// stop listening to the partition changed event
		if ( CameraManager.Instance )
			CameraManager.Instance.GetPanScript().OnPartitionChanging -= OnPartitionChanging;
    }
	
	protected override void _ExtendedAction(GameObject emittedObject){
		// set the proper move script
		MoveTowards moveScript = emittedObject.GetComponent<MoveTowards>();
		if(moveScript != null){
			moveScript.Target = targetDestination.transform;
			moveScript.touchCallbackTarget = gameObject;
		}
		else{
			Debug.LogError("No MoveTowards script detected in particle");
		}
		
		// set the proper degrad particle script
		DegradParticle scriptParticle = emittedObject.GetComponent<DegradParticle>();
		int nDamage = GetDamage();
		scriptParticle.SetDamage( nDamage );
		
		// add the tracking script to track the emitted particles
		TrackObject scriptTrack = emittedObject.AddComponent<TrackObject>();
		scriptTrack.Init( listSpawned );
		
	}
	
	///////////////////////////////////////////
	// OnPartitionChanging()
	// Degrad emitters need to know about
	// partition changes so they can stop
	// spawning particles when changing to a
	// gated room.
	///////////////////////////////////////////	
	private void OnPartitionChanging( object sender, PartitionChangedArgs args ) {
		// find out if the room being changed to has a gate or not
		int nEntering = args.nNew;
		bool bGated = false;
		
		if ( GatingManager.Instance )
			GatingManager.Instance.HasActiveGate( nEntering );		
		
		if ( bGated ) {
			// if there is a gate in this room, turn off the spawning of skulls
			Disable();
			
			// also, all spawned objects need to get to their target asap
			for ( int i = 0; i < listSpawned.Count; ++i ) {
				GameObject go = listSpawned[i];
				MoveTowards moveScript = go.GetComponent<MoveTowards>();
				moveScript.FlashToTarget();
			}
		}
		else {
			// if there is no gated in this room, we can spawn skulls again (but only if the tut is through)
			if ( !TutorialLogic.Instance.FirstTimeDegradTrigger && !isActive )
				Enable();
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
}
