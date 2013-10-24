using UnityEngine;
using System;
using System.Collections;

public class DegradTrigger : MonoBehaviour {
    public int ID{get; set;} 		//the id of this specific degradation trigger
	public string strSoundClean;	// sound this degrade trigger makes when the player cleans it up
	public GameObject LgDegredationEmitter;		// The custom emittor emitter that emits skulls that fly towards pet
	private LgParticleEmitterDegredation emitter;
	
	// has this trigger been cleaned yet?
	private bool bCleaned = false;
	
	// Use this for initialization
	void Start(){
		// set the emitter
		emitter = LgDegredationEmitter.GetComponent<LgParticleEmitterDegredation>();
		
		// set emitter values
		float fTime = Constants.GetConstant<float>( "DegradTickTime" );
		int nDamage = Constants.GetConstant<int>( "DegradDamage" );
		emitter.InitDegradEmitter( fTime, nDamage );
		
        DegradationUIManager.OnActivateParticleEffects += ActivateParticleEffects;
            
        //Disable particle effects when other tutorials are not finished yet
        if(TutorialLogic.Instance.FirstTimeRealInhaler || 
            TutorialLogic.Instance.FirstTimeCalendar){
            transform.Find("SkullParticle").GetComponent<ParticleSystem>().Stop();
        }
		
		// Attach the particle end location to the pet's hit position
		emitter.targetDestination = DegradationUIManager.Instance.petHitLocation;
		
		// listen for when the player changes partitions, as this trigger may need to act
		CameraManager.Instance.GetPanScript().OnPartitionChanging += OnPartitionChanging;
	}

    void OnDestroy(){
        DegradationUIManager.OnActivateParticleEffects -= ActivateParticleEffects;
		
		// stop listening to the partition changed event
		if ( CameraManager.Instance )
			CameraManager.Instance.GetPanScript().OnPartitionChanging -= OnPartitionChanging;
    }
	
	private void OnPartitionChanging( object sender, PartitionChangedArgs args ) {
		// find out if the room being changed to has a gate or not
		int nEntering = args.nNew;
		bool bGated = GatingManager.Instance.HasActiveGate( nEntering );		
		
		if ( bGated ) {
			// if there is a gate in this room, turn off the spawning of skulls
			emitter.Disable();
		}
		else {
			// if there is no gated in this room, we can spawn skulls again (but only if the tut is through)
			if ( !TutorialLogic.Instance.FirstTimeDegradTrigger )
				emitter.Enable();
		}
	}

    //Listen to OnTap event from FingerGesture
    void OnTap(TapGesture gesture){
		// check if this trigger has been cleaned yet
		if ( bCleaned )
			return;
		
        //when trigger is touched remove from DataManager and destroy GameObject
        if(ClickManager.Instance.CanRespondToTap()){
			// play sound associated with cleaning the trigger
			AudioManager.Instance.PlayClip( strSoundClean );
			
            if(TutorialLogic.Instance.FirstTimeDegradTrigger)
               TutorialUIManager.Instance.StartDegradTriggerTutorial();
            CleanTriggerAndDestroy();
        }
    }

    private void CleanTriggerAndDestroy(){
		bCleaned = true;
        DegradationLogic.Instance.ClearDegradationTrigger(ID);
        Destroy(this.gameObject, 0.5f);
    }

    private void ActivateParticleEffects(object senders, EventArgs args){
        transform.Find("SkullParticle").GetComponent<ParticleSystem>().Play();
		
		// Enable the skull flying to pet
		emitter.Enable();
    }
}
