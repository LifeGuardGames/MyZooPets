using UnityEngine;
using System.Collections;

public class InhalerGameManager : MonoBehaviour{

    public GameObject advairPrefab;
    public GameObject rescuePrefab;
    public GameObject rescueShakerPrefab; // arrows that indicate that the rescue inhaler has to be shaken
    public GameObject inhaleExhalePrefab; // arrows that indicate whether to breathe in or out
    public GameObject smallRescuePrefab; // rescue inhaler that appears in front of the pet's mouth

    public GameObject slotMachine;

    private GameObject advair;
    private GameObject rescue;
    private GameObject rescueShaker; // arrows that indicate that the rescue inhaler has to be shaken
    private GameObject inhaleExhale; // arrows that indicate whether to breathe in or out
    private GameObject smallRescue; // rescue inhaler that appears in front of the pet's mouth

    private SlotMachineManager slotMachineManager; // component of slotMachine

    public void ResetInhalerGame(){
        DestroyAndRecreatePrefabs();
        SetUpInhalerGame();
    }

    // On Awake, initialize the values in InhalerLogic. Then determine whether to show (activate)
    // the Advair inhaler or Rescue inhaler, depending on what InhalerLogic.CurrentInhalerType is
    void Awake(){
        ResetInhalerGame();
    }
    void Start(){

    }

    void DestroyAndRecreatePrefabs(){
        // delete gameobjects
        Destroy(advair);
        Destroy(rescue);
        Destroy(smallRescue);
        Destroy(rescueShaker);
        Destroy(inhaleExhale);

        // instantiate new prefabs and store references to new gameobjects
        advair = Instantiate(advairPrefab) as GameObject;
        advair.name = advairPrefab.name;
        smallRescue = Instantiate(smallRescuePrefab) as GameObject;
        smallRescue.name = smallRescuePrefab.name;

        rescue = Instantiate(rescuePrefab) as GameObject;
        rescue.name = rescuePrefab.name;
        rescue.GetComponent<RescueBody>().miniature = smallRescue;

        rescueShaker = Instantiate(rescueShakerPrefab) as GameObject;
        rescueShaker.name = rescueShakerPrefab.name;
        inhaleExhale = Instantiate(inhaleExhalePrefab) as GameObject;
        inhaleExhale.name = inhaleExhalePrefab.name;

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
            InhalerLogic.Init();
        }
    }

    // void InvokeSlotMachine(){
    //     Invoke("ShowSlotMachine", 3);
    // }

    void ShowSlotMachine(){
        slotMachine.SetActive(true);
        if (!slotMachineManager.SpinWhenReady()){
            // show only for a short while, then ResetInhalerGame()
            Invoke("ResetInhalerGame", 3);
        }
    }
}
