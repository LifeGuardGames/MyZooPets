using UnityEngine;
using System;
using System.Collections;

public class DegradTrigger : MonoBehaviour {
    public int ID{get; set;} //the id of this specific degradation trigger

	// Use this for initialization
	void Start () {
        DegradationUIManager.OnActivateParticleEffects += ActivateParticleEffects;
            
        //Disable particle effects when other tutorials are not finished yet
        if(TutorialLogic.Instance.FirstTimeRealInhaler || 
            TutorialLogic.Instance.FirstTimeCalendar){
            transform.Find("SkullParticle").GetComponent<ParticleSystem>().Stop();
        }
	}

    void OnDestroy(){
        DegradationUIManager.OnActivateParticleEffects -= ActivateParticleEffects;
    }

    //Listen to OnTap event from FingerGesture
    void OnTap(TapGesture gesture){
        //when trigger is touched remove from DataManager and destroy GameObject
        if(ClickManager.Instance.CanRespondToTap()){
            if(TutorialLogic.Instance.FirstTimeDegradTrigger)
                TutorialUIManager.Instance.StartDegradTriggerTutorial();
            CleanTriggerAndDestroy();
        }
    }

    private void CleanTriggerAndDestroy(){
        DegradationLogic.Instance.ClearDegradationTrigger(ID);
        Destroy(this.gameObject, 0.5f);
    }

    private void ActivateParticleEffects(object senders, EventArgs args){
        transform.Find("SkullParticle").GetComponent<ParticleSystem>().Play();
    }
}
