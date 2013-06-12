using UnityEngine;
using System.Collections;

public class InhalerGameManager : MonoBehaviour{

    public GameObject advair;
    public GameObject rescue;

    void Awake(){
        InhalerLogic.Init();

        // todo: remove after testing
        InhalerLogic.CurrentInhalerType = InhalerType.Rescue;

        Debug.Log("Current inhaler type is -> " + InhalerLogic.CurrentInhalerType);
        if (InhalerLogic.CurrentInhalerType == InhalerType.Advair){
            rescue.SetActive(false);
        }
        else if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue){
            advair.SetActive(false);
        }
    }
}
