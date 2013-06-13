using UnityEngine;
using System.Collections;

public class InhalerGameManager : MonoBehaviour{

    public GameObject advairPrefab;
    public GameObject rescuePrefab;
    public GameObject smallRescuePrefab; // rescue inhaler that appears in front of the pet's mouth
    public GameObject rescueShakerPrefab; // arrows that indicate that the rescue inhaler has to be shaken
    public GameObject inhaleExhalePrefab; // arrows that indicate whether to breathe in or out

    public GameObject slotMachine;

    private GameObject advair;
    private GameObject rescue;
    private GameObject smallRescue; // rescue inhaler that appears in front of the pet's mouth
    private GameObject rescueShaker; // arrows that indicate that the rescue inhaler has to be shaken
    public GameObject inhaleExhale; // arrows that indicate whether to breathe in or out

    private SlotMachineManager slotMachineManager; // component of slotMachine

    public void ResetInhalerGame(){
        // delete gameobjects
        Destroy(advair);
        Destroy(rescue);
        Destroy(smallRescue);
        Destroy(rescueShaker);
        Destroy(inhaleExhale);

        // instantiate new prefabs and store references to new gameobjects
        advair = Instantiate(advairPrefab) as GameObject;
        rescue = Instantiate(rescuePrefab) as GameObject;
        smallRescue = Instantiate(smallRescuePrefab) as GameObject;
        rescueShaker = Instantiate(rescueShakerPrefab) as GameObject;
        inhaleExhale = Instantiate(inhaleExhalePrefab) as GameObject;

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

        slotMachineManager = slotMachine.GetComponent<SlotMachineManager>();
        // hide slot machine
        slotMachine.SetActive(false);

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
            InhalerLogic.ResetGame(); // call this before showing the slots
            ShowSlotMachine();
        }
    }

    void ShowSlotMachine(){
        slotMachine.SetActive(true);
        // todo
        // if slot machine count == 3
        slotMachineManager.StartGame();
    }
}
