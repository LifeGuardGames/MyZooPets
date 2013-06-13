using UnityEngine;
using System.Collections;

public class InhalerGameManager : MonoBehaviour{

    public GameObject advair;
    public GameObject rescue;
    public GameObject smallRescue; // rescue inhaler that appears in front of the pet's mouth
    public GameObject rescueShaker; // arrows that indicate that the rescue inhaler has to be shaken

    public void ResetInhalerGame(){
        // reset inhaler prefabs

        // hide slot machine

        SetUpInhalerGame();
    }

    // On Awake, initialize the values in InhalerLogic. Then determine whether to show (activate)
    // the Advair inhaler or Rescue inhaler, depending on what InhalerLogic.CurrentInhalerType is
    void Awake(){
        SetUpInhalerGame();
    }

    void SetUpInhalerGame(){
        InhalerLogic.Init();

        // todo: remove after testing
        // InhalerLogic.CurrentInhalerType = InhalerType.Rescue;

        Debug.Log("Current inhaler type is -> " + InhalerLogic.CurrentInhalerType);
        if (InhalerLogic.CurrentInhalerType == InhalerType.Advair){
            rescue.SetActive(false);
            rescueShaker.SetActive(false);
        }
        else if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue){
            advair.SetActive(false);
        }
        smallRescue.SetActive(false);
    }


    void Update(){
        if (InhalerLogic.IsDoneWithGame()){ // if done with game
            ShowSlotMachine();
        }
    }

    void ShowSlotMachine(){

    }
}
