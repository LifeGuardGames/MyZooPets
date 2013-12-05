using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DegradTrigger : MonoBehaviour {
    public int ID{get; set;} 		//the id of this specific degradation trigger
	public string strSoundClean;	// sound this degrade trigger makes when the player cleans it up
	public GameObject LgDegredationEmitter;		// The custom emittor emitter that emits skulls that fly towards pet
	private LgParticleEmitterDegredation emitter;
	
    //=======================Events========================
    public EventHandler<EventArgs> OnTriggerCleaned;   // when this trigger gets cleaned
	
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
		
		// deprecated stuff below...with the new tutorials, we activate particle effects differently for triggers.
		// remove this when it seems safe to so.
        //DegradationUIManager.OnActivateParticleEffects += ActivateParticleEffects;
            
        //Disable particle effects when other tutorials are not finished yet
        /*if(TutorialLogic.Instance.FirstTimeRealInhaler || 
            TutorialLogic.Instance.FirstTimeCalendar){
            transform.Find("SkullParticle").GetComponent<ParticleSystem>().Stop();
        }*/
		
		// Attach the particle end location to the pet's hit position
		emitter.targetDestination = DegradationUIManager.Instance.petHitLocation;
		
		// if the trigger tutorial has been played, activate the trigger
		bool bTriggers = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TutorialManager_Bedroom.TUT_TRIGGERS );
		if ( bTriggers || DegradationUIManager.Instance.IsTesting() )
			ActivateParticles();		
	}

    void OnDestroy(){
        DegradationUIManager.OnActivateParticleEffects -= ActivateParticleEffects;
    }

    //Listen to OnTap event from FingerGesture
    void OnTap(TapGesture gesture){
		// check if this trigger has been cleaned yet
		if ( bCleaned )
			return;
		
        //when trigger is touched remove from DataManager and destroy GameObject
        if(ClickManager.Instance.CanRespondToTap( gameObject )){
			// play sound associated with cleaning the trigger
			AudioManager.Instance.PlayClip( strSoundClean );
			
            // if(TutorialLogic.Instance.FirstTimeDegradTrigger)
            //    TutorialUIManager.Instance.StartDegradTriggerTutorial();
            CleanTriggerAndDestroy();
        }
    }

    private void CleanTriggerAndDestroy(){
		bCleaned = true;
        DegradationLogic.Instance.ClearDegradationTrigger(ID);
		
		// send out callback
		if ( OnTriggerCleaned != null )
			OnTriggerCleaned( this, EventArgs.Empty );			
		
        Destroy(this.gameObject, 0.5f);
    }

    private void ActivateParticleEffects(object senders, EventArgs args){
		ActivateParticles();
    }

	private void ActivateParticles() {
       transform.Find("SkullParticle").GetComponent<ParticleSystem>().Play();
		
		// Enable the skull flying to pet
		emitter.Enable();		
	}
	
	//---------------------------------------------------
	// FireOneSkull()
	// This is a somewhat hacky function used for the
	// trigger tutorial that just fires one skull object,
	// then turns off.
	//---------------------------------------------------	
	public IEnumerator FireOneSkull() {
		// enable the emitter so it shoots a skull
		emitter.Enable( true );
		
		// wait a frame
		yield return 0;
		
		// now disable the emitter
		emitter.Disable();
	}
}
