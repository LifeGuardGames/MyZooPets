using UnityEngine;
using System;
using System.Collections;

public class DegradTrigger : MonoBehaviour {
    public int ID{get; set;} 		//the id of this specific degradation trigger
	public string strSoundClean;	// sound this degrade trigger makes when the player cleans it up
	public GameObject LgDegredationEmitter;		// The custom emittor emitter that emits skulls that fly towards pet
	
	// has this trigger been cleaned yet?
	private bool bCleaned = false;
	
	// Use this for initialization
	void Start(){
        DegradationUIManager.OnActivateParticleEffects += ActivateParticleEffects;
            
        //Disable particle effects when other tutorials are not finished yet
        if(TutorialLogic.Instance.FirstTimeRealInhaler || 
            TutorialLogic.Instance.FirstTimeCalendar){
            transform.Find("SkullParticle").GetComponent<ParticleSystem>().Stop();
        }
		
		// Attach the particle end location to the pet's hit position
		LgParticleEmitterDegredation emitter = LgDegredationEmitter.GetComponent<LgParticleEmitterDegredation>();
		emitter.targetDestination = DegradationUIManager.Instance.petHitLocation;
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
		LgDegredationEmitter.GetComponent<LgParticleEmitterDegredation>().Enable();
    }
}
