using UnityEngine;
using System;
using System.Collections;

public class DegradTriggerManager : MonoBehaviour {
    public int id; //the id of this specific degradation trigger
    private DegradationLogic degradationLogic;

	// Use this for initialization
	void Start () {
        degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
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
        if (ClickManager.CanRespondToTap()){
            degradationLogic.ClearDegradationTrigger(id);
            Destroy(this.gameObject);
        }
    }

    private void ActivateParticleEffects(object senders, EventArgs args){
        transform.Find("SkullParticle").GetComponent<ParticleSystem>().Play();
    }
}
