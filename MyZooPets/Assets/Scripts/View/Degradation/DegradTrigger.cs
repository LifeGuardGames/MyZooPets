using UnityEngine;
using System;
using System.Collections;

public class DegradTrigger : MonoBehaviour{
	public EventHandler<EventArgs> OnTriggerCleaned;	// when this trigger gets cleaned

	public string ID { get; set; }						//the id of this specific degradation trigger
	public LgParticleEmitterDegradation emitter;
	public ParticleSystem hitParticle;
	private int clicksToKill = 3;

	// has this trigger been cleaned yet?
	private bool isCleaned = false;
	
	// Use this for initialization
	void Start(){
		emitter.InitDegradEmitter(10f, 1, DegradationUIManager.Instance.petHitLocation.transform);

		// if the trigger tutorial has been played, activate the trigger
		bool areTutorialsFinished = DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished();
		if(areTutorialsFinished || DegradationLogic.Instance.IsTesting()) {
			ActivateParticles();		
		}
	}

	void OnDestroy(){
		DegradationUIManager.OnActivateParticleEffects -= ActivateParticleEffects;
	}

	//Listen to OnTap event from FingerGesture
	void OnTap(TapGesture gesture){
		// check if this trigger has been cleaned yet
		if(isCleaned){
			return;
		}
		
		AudioManager.Instance.PlayClip("hit");
		hitParticle.Play();

		if(--clicksToKill > 0){
			LeanTween.moveLocalX(gameObject, 1f, 0.3f).setEase(LeanTweenType.punch);
		}
		else{
			//when trigger is killed remove from DataManager and destroy GameObject
			if(ClickManager.Instance.CanRespondToTap(gameObject)){
				AudioManager.Instance.PlayClip("CleanDust");
				CleanTriggerAndDestroy();
			}
		}
	}

	private void CleanTriggerAndDestroy(){
		isCleaned = true;
		DegradationLogic.Instance.ClearDegradationTrigger(this);
		
		// send out callback
		if(OnTriggerCleaned != null){
			OnTriggerCleaned(this, EventArgs.Empty);		
		}
		
		// play an FX
		ParticleUtils.CreateParticle("DegradationPoof", gameObject.transform.position);
		
		Destroy(gameObject);
	}

	private void ActivateParticleEffects(object senders, EventArgs args){
		ActivateParticles();
	}

	private void ActivateParticles(){
		transform.Find("SkullParticle").GetComponent<ParticleSystem>().Play();
		emitter.Enable();		// Enable the skull flying to pet	
	}

	/// <summary>
	/// This is a somewhat hacky function used for the trigger tutorial that just fires one skull object,
	/// then turns off.
	/// </summary>
	public IEnumerator FireOneSkull(){
		emitter.Enable(true);	// enable the emitter so it shoots a skull
		yield return 0;
		emitter.Disable();
	}
}
