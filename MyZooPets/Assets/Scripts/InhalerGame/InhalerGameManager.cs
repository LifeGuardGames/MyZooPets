using UnityEngine;
using System.Collections;

public class InhalerGameManager : MonoBehaviour{

    public GameObject advair;
    public GameObject rescue;

    void Start(){
        InhalerLogic.Init();
        Debug.Log("Current inhaler type is -> " + InhalerLogic.CurrentInhalerType);
        if (InhalerLogic.CurrentInhalerType == InhalerType.Advair){
            rescue.SetActive(false);
        }
        else if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue){
            advair.SetActive(false);
        }
    }
}
