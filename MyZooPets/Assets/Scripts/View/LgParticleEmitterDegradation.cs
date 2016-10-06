using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LgParticleEmitterDegradation : LgParticleEmitter{

	private Transform targetDestination;
	private List<GameObject> listSpawned = new List<GameObject>(); // list of objects this emitter has spawned
	private bool isSubscribed = false; // is this trigger subscribed to the partition changing event?

	// amount of damage the emitter particles cause the pet's health when they collide
	private int damage;

	private int GetDamage(){
		return damage;	
	}
	
	protected override void OnEnabled(bool bImmediate){
		// since the emitter is being enabled, it now needs to subscribe to events
		Subscribe(true);
	}
	
	///////////////////////////////////////////
	// Subscribe()
	// This function handles the subscription
	// and unsubscription for whatever events
	// this emitter should listen to.
	// Right now it is just partition changing.
	///////////////////////////////////////////	
	private void Subscribe(bool subscribeNow){
		if(subscribeNow && !isSubscribed){
			isSubscribed = true; 	
			CameraManager.Instance.PanScript.OnPartitionChanging += OnPartitionChanging;	
		}
		else if(!subscribeNow && isSubscribed && CameraManager.Instance){
			isSubscribed = false;	
			CameraManager.Instance.PanScript.OnPartitionChanging -= OnPartitionChanging;
		}
	}
	
	protected override void OnDestroy(){
		base.OnDestroy();
		// since the emitter is being destroyed, time to unsubscribe to events
		Subscribe(false);
	}
	
	protected override void ExtendedAction(GameObject emittedObject){
		// set the proper move script
		MoveTowards moveScript = emittedObject.GetComponent<MoveTowards>();
		if(moveScript != null){
			moveScript.Target = targetDestination;
			moveScript.touchCallbackTarget = gameObject;
		}
		else{
			Debug.LogError("No MoveTowards script detected in particle");
		}
		
		// set the proper degrad particle script
		DegradParticle scriptParticle = emittedObject.GetComponent<DegradParticle>();
		int nDamage = GetDamage();
		scriptParticle.Damage = nDamage;
		
		// add the tracking script to track the emitted particles
//		TrackObject scriptTrack = emittedObject.AddComponent<TrackObject>();
//		scriptTrack.Init(listSpawned);
		
	}
	
	///////////////////////////////////////////
	// OnPartitionChanging()
	// Degrad emitters need to know about
	// partition changes so they can stop
	// spawning particles when changing to a
	// gated room.
	///////////////////////////////////////////	
	private void OnPartitionChanging(object sender, PartitionChangedArgs args){
		// find out if the room being changed to has a gate or not
		int nEntering = args.newPartition;
		bool bGated = false;
		
		if(GatingManager.Instance)
			bGated = GatingManager.Instance.HasActiveGate(nEntering);		
		
		if(bGated){
			// if there is a gate in this room, turn off the spawning of skulls
			Disable();
			
			// also, all spawned objects need to get to their target asap
			for(int i = 0; i < listSpawned.Count; ++i){
				GameObject go = listSpawned[i];
				MoveTowards moveScript = go.GetComponent<MoveTowards>();
				moveScript.FlashToTarget();
			}
		}
		else{
			// if there is no gate in this room, we can spawn skulls again (but only if the tut is through)
			if(!isActive)
				Enable();
		}
	}	
	
	///////////////////////////////////////////
	// InitDegradEmitter()
	// Sets some basic values for the emitter
	// and what kind of damage it causes.
	///////////////////////////////////////////		
	public void InitDegradEmitter(float fTime, int nDamage, Transform targetDest){
		minInterval = fTime;
		maxInterval = fTime;
		this.damage = nDamage;
		targetDestination = targetDest;
	}
}
