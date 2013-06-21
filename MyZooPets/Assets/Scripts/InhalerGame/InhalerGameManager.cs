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
    InhalerGameGUI inhalerGameGUI;

    // todo: create accessors
    public bool showPlayAgain = false;
    public bool gameEnded = false;

    int lastRecordedStep;
    public bool ShowHint{
        get {return showHint;}
    }
    bool showHint = false;
    bool runShowHintTimer = true;
    float timer = 0;
    float timeBeforeHints = 5.0f;

    public void ResetInhalerGame(){
        if (InhalerLogic.CanPlayGame){ // tells us if we can play the game or not (any more plays remaining today)
            DestroyAndRecreatePrefabs();
            SetUpInhalerGame();
        }
        else {
            slotMachine.SetActive(false);
        }
        gameEnded = false;
        showPlayAgain = false;
    }

    // On Awake, initialize the values in InhalerLogic. Then determine whether to show (activate)
    // the Advair inhaler or Rescue inhaler, depending on what InhalerLogic.CurrentInhalerType is
    void Awake(){
        ResetInhalerGame();
    }
    void Start(){

        inhalerGameGUI = GameObject.Find("InhalerGameGUI").GetComponent<InhalerGameGUI>();
        slotMachineManager.SpinEndCallBack = FinishedSpinning;
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
        rescueShaker.GetComponent<RescueShaker>().rescueBody = rescue.GetComponent<RescueBody>();

        inhaleExhale = Instantiate(inhaleExhalePrefab) as GameObject;
        inhaleExhale.name = inhaleExhalePrefab.name;

    }

    void SetUpInhalerGame(){
        InhalerLogic.Init(false);

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

        if ((InhalerLogic.CurrentInhalerType == InhalerType.Advair && DataManager.FirstTimeAdvair) ||
            (InhalerLogic.CurrentInhalerType == InhalerType.Rescue && DataManager.FirstTimeRescue)){
            runShowHintTimer = false;
            showHint = true;
        }
        else {
            runShowHintTimer = true;
        }

    }

    void Update(){
        if (runShowHintTimer){
            ShowHintTimer();
        }
    }

    void ShowHintTimer(){ // to be called in Update()
        if (InhalerLogic.CurrentStep != lastRecordedStep){
            timer = 0;
            showHint = false;
            lastRecordedStep = InhalerLogic.CurrentStep;
        }
        else {
            timer += Time.deltaTime;
            if (timer > timeBeforeHints){
                showHint = true;
            }
        }
    }

    public void OnGameEnd(){
        if (InhalerLogic.IsDoneWithGame()){ // if done with game
            inhalerGameGUI.DisplayMessage();
            RemoveFirstTimeFlags();
            gameEnded = true;
            // InhalerLogic.ResetGame(); // call this before showing the slots
            inhalerGameGUI.HideButtons();
            Invoke("ShowSlotMachine", 3); // set a 3 second delay so that the "great" message animation has time to play
        }
    }

    void RemoveFirstTimeFlags(){
        if (InhalerLogic.CurrentInhalerType == InhalerType.Advair && DataManager.FirstTimeAdvair){
            DataManager.FirstTimeAdvair = false;
        }
        else if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue && DataManager.FirstTimeRescue){
            DataManager.FirstTimeRescue = false;
        }
    }

    void ShowSlotMachine(){
        slotMachine.SetActive(true);
        slotMachineManager.SpinWhenReady();
    }

    void FinishedSpinning(){
        showPlayAgain = true;
        inhalerGameGUI.ShowButtons();
        if (slotMachineManager.CheckMatch()){
            // todo: change later
            DataManager.AddPoints(100);
        }
    }
}
