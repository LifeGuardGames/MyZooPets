using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Instantiate all the degradation asthma triggers if there are any
public class DegradationGUI : MonoBehaviour{
    public GameObject cleanTriggerParticleDrop;

    /*
        handle particle system stuff in this class
        perhaps a callback from DegradationLogic is required
    */

    private DegradationLogic degradationLogic;

    void Start(){
        degradationLogic = GameObject.Find("GameManager").GetComponent<DegradationLogic>();
        degradationLogic.TriggerDestroyed += SpawnStarsWhenTriggersDestroyed;
    }

    public void Init(){
        //instantiate triggers in the game
        for(int i=0; i<DataManager.DegradationTriggers.Count; i++){
            int prefabId = DataManager.DegradationTriggers[i].PrefabId;
            int positionId = DataManager.DegradationTriggers[i].PositionId;

            //instantiate all the triggers save in DataManager
            GameObject trigger = (GameObject)Instantiate(degradationLogic.triggerPrefabs[prefabId],
                degradationLogic.triggerLocations[positionId].position, Quaternion.identity);
            trigger.GetComponent<DegradTriggerManager>().id = i;
        }
    }

    private void SpawnStarsWhenTriggersDestroyed(object sender, DegradationLogic.TriggerDestroyedEventArgs e){
        GameObject particleDrop = (GameObject)Instantiate(cleanTriggerParticleDrop, e.TriggerPosition, Quaternion.Euler(270,0,0));
        Destroy(particleDrop, 1);
    }
}