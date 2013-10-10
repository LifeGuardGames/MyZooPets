using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Instantiate all the degradation asthma triggers if there are any
public class DegradationUIManager : Singleton<DegradationUIManager>{
    //When particle effects need to be turned on
    public static event EventHandler<EventArgs> OnActivateParticleEffects;
    public GameObject cleanTriggerParticleDrop;
	
	public GameObject petHitLocation;	// Used for triggers to attach as end destination

    void Start(){
        //instantiate triggers in the game
        List<DegradData> degradTriggers = DegradationLogic.Instance.DegradationTriggers;
        List<GameObject> triggerPrefabs = DegradationLogic.Instance.TriggerPrefabs;
        List<DegradationLogic.Location> triggerLocations = DegradationLogic.Instance.TriggerLocations;

        for(int i=0; i<degradTriggers.Count; i++){
            int prefabId = degradTriggers[i].PrefabId;
            int positionId = degradTriggers[i].PositionId;
            //instantiate all the triggers save in DataManager
            GameObject trigger = (GameObject)Instantiate(triggerPrefabs[prefabId],
                triggerLocations[positionId].position, Quaternion.identity);
            trigger.GetComponent<DegradTrigger>().ID = i;
        }
    }

    //Use this to turn on all particle effects in triggers
    public void ActivateParticleEffects(){
        if(OnActivateParticleEffects != null)
            OnActivateParticleEffects(this, EventArgs.Empty);
        else
            Debug.LogWarning("OnActivateParticleEffects is null");
    }
}