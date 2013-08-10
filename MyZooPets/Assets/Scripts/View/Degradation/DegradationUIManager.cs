using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Instantiate all the degradation asthma triggers if there are any
public class DegradationUIManager : Singleton<DegradationUIManager>{
    public GameObject cleanTriggerParticleDrop;
    private DegradationLogic degradationLogic;

    //=========================Events===================================
    //When particle effects need to be turned on
    public static event EventHandler<EventArgs> OnActivateParticleEffects;
    //==================================================================

    void Awake(){
        degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
    }

    void Start(){
        //instantiate triggers in the game
        for(int i=0; i<degradationLogic.DegradationTriggers.Count; i++){
            int prefabId = degradationLogic.DegradationTriggers[i].PrefabId;
            int positionId = degradationLogic.DegradationTriggers[i].PositionId;
            //instantiate all the triggers save in DataManager
            GameObject trigger = (GameObject)Instantiate(degradationLogic.triggerPrefabs[prefabId],
                degradationLogic.triggerLocations[positionId].position, Quaternion.identity);
            trigger.GetComponent<DegradTriggerManager>().id = i;
        }
    }

    //Use this to turn on all particle effects in triggers
    public void ActivateParticleEffects(){
        if(D.Assert(OnActivateParticleEffects != null, "OnActivateParticleEffetcts has no listeners"))
            OnActivateParticleEffects(this, EventArgs.Empty);
    }
}