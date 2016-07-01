using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DegradTrigger : MonoBehaviour{
	public string ID{ get; set; } 		//the id of this specific degradation trigger
	public string CleanSound;	// sound this degrade trigger makes when the player cleans it up
	public ParticleSystem hitParticle;
	public GameObject LgDegradationEmitter;		// The custom emittor emitter that emits skulls that fly towards pet
	private LgParticleEmitterDegradation emitter;
	private int clicksToKill;
	
	//=======================Events========================
	public EventHandler<EventArgs> OnTriggerCleaned;   // when this trigger gets cleaned
	
	// has this trigger been cleaned yet?
	private bool isCleaned = false;
	
	// Use this for initialization
	void Start(){
		// Set life of the emitter
		clicksToKill = Constants.GetConstant<int>("DegradHitsToClean");

		// set the emitter
		emitter = LgDegradationEmitter.GetComponent<LgParticleEmitterDegradation>();
		// set emitter values
		float fTime = Constants.GetConstant<float>("DegradTickTime");
		int nDamage = Constants.GetConstant<int>("DegradDamage");
		emitter.InitDegradEmitter(fTime, nDamage);
		// Attach the particle end location to the pet's hit position
		emitter.targetDestination = DegradationUIManager.Instance.petHitLocation;
		
		// if the trigger tutorial has been played, activate the trigger
		bool areTutorialsFinished = DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished();
		if(areTutorialsFinished || DegradationUIManager.Instance.IsTesting()){
			ActivateParticles();		
		}
	}

	void OnDestroy(){
		DegradationUIManager.OnActivateParticleEffects -= ActivateParticleEffects;
	}

	//Listen to OnTap event from FingerGesture
	void OnTap(TapGesture gesture){
		// check if this trigger has been cleaned yet
		if(isCleaned)
			return;
		
		AudioManager.Instance.PlayClip("hit");
		hitParticle.Play();

		if(--clicksToKill > 0){
			LeanTween.moveLocalX(gameObject, 1f, 0.3f).setEase(LeanTweenType.punch);
		}
		else{
			//when trigger is killed remove from DataManager and destroy GameObject
			if(ClickManager.Instance.CanRespondToTap(gameObject)){
				// play sound associated with cleaning the trigger
				AudioManager.Instance.PlayClip(CleanSound);
				
				CleanTriggerAndDestroy();
			}
		}
	}

	private void CleanTriggerAndDestroy(){
		isCleaned = true;
		DegradationLogic.Instance.ClearDegradationTrigger(this);
		
		// send out callback
		if(OnTriggerCleaned != null)
			OnTriggerCleaned(this, EventArgs.Empty);		
		
		// play an FX
		Vector3 vPosFX = gameObject.transform.position;
		string strFX = Constants.GetConstant<string>("DegradCleanParticle");
		ParticleUtils.CreateParticle(strFX, vPosFX);		
		
		Destroy(this.gameObject);
	}

	private void ActivateParticleEffects(object senders, EventArgs args){
		ActivateParticles();
	}

	private void ActivateParticles(){
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
	public IEnumerator FireOneSkull(){
		// enable the emitter so it shoots a skull
		emitter.Enable(true);
		
		// wait a frame
		yield return 0;
		
		// now disable the emitter
		emitter.Disable();
	}
}
