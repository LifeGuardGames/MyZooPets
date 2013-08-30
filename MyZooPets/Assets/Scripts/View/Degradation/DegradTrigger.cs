using UnityEngine;
using System;
using System.Collections;

public class DegradTrigger : MonoBehaviour {
    public int ID{get; set;} //the id of this specific degradation trigger

	// Use this for initialization
	void Start () {
        GetComponent<TapItem>().OnTap += OnTap;
        DegradationUIManager.OnActivateParticleEffects += ActivateParticleEffects;

       //Set up for tutorial
        if(TutorialLogic.Instance.FirstTimeDegradTrigger)
            GetComponent<TapItem>().OnTap += TutorialUIManager.Instance.StartDegradTriggerTutorial;
            
        //Disable particle effects when other tutorials are not finished yet
        if(TutorialLogic.Instance.FirstTimeRealInhaler || 
            TutorialLogic.Instance.FirstTimeCalendar){
            transform.Find("SkullParticle").GetComponent<ParticleSystem>().Stop();
        }
	}

    void OnDestroy(){
        DegradationUIManager.OnActivateParticleEffects -= ActivateParticleEffects;
    }

    private void OnTap(){
        //when trigger is touched remove from DataManager and destroy GameObject
        if (ClickManager.Instance.CanRespondToTap()){
            DegradationLogic.Instance.ClearDegradationTrigger(ID);
            Destroy(this.gameObject);
        }
    }

    private void ActivateParticleEffects(object senders, EventArgs args){
        transform.Find("SkullParticle").GetComponent<ParticleSystem>().Play();
    }
}
