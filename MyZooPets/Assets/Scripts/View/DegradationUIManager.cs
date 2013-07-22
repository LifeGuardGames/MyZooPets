using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Instantiate all the degradation asthma triggers if there are any
public class DegradationUIManager : MonoBehaviour{
    public GameObject cleanTriggerParticleDrop;
    private DegradationLogic degradationLogic;

    void Awake(){
        degradationLogic = GameObject.Find("GameManager/DegradationLogic").GetComponent<DegradationLogic>();
    }

    void Start(){
        DegradationLogic.OnTriggerDestroyed += SpawnStarsWhenTriggersDestroyed;

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

    void OnDestroy(){
        DegradationLogic.OnTriggerDestroyed -= SpawnStarsWhenTriggersDestroyed;
    }

    private void SpawnStarsWhenTriggersDestroyed(object sender, DegradationLogic.TriggerDestroyedEventArgs e){
        GameObject particleDrop = (GameObject)Instantiate(cleanTriggerParticleDrop, e.TriggerPosition, Quaternion.Euler(270,0,0));
        Destroy(particleDrop, 1);
    }
}