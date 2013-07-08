using UnityEngine;
using System.Collections;

public class InhalerGameManager : MonoBehaviour{

    public GameObject advairPrefab;
    public GameObject rescuePrefab;
    public GameObject rescueShakerPrefab; // arrows that indicate that the rescue inhaler has to be shaken
    public GameObject inhaleExhalePrefab; // arrows that indicate whether to breathe in or out
    public GameObject smallRescuePrefab; // rescue inhaler that appears in front of the pet's mouth

    private GameObject advair;
    private GameObject rescue;
    private GameObject rescueShaker; // arrows that indicate that the rescue inhaler has to be shaken
    private GameObject inhaleExhale; // arrows that indicate whether to breathe in or out
    private GameObject smallRescue; // rescue inhaler that appears in front of the pet's mouth

    public bool isPracticeGame;

    int practiceGamePointIncrement = 50;
    public int PracticeGamePointIncrement {
        get {return practiceGamePointIncrement;}
    }

    bool hasPlayedGame = false;
    public bool HasPlayedGame{
        get {return hasPlayedGame;}
    }

    InhalerGameGUI inhalerGameGUI;

    // todo: create accessors
    public bool gameEnded = false;

    int lastRecordedStep;
    public bool ShowHint{
        get {return showHint;}
    }
    bool showHint = false;
    bool runShowHintTimer = true;
    float timer = 0;
    float timeBeforeHints = 5.0f;
    bool introShown = false;

    void Start(){
        inhalerGameGUI = GameObject.Find("InhalerGameGUI").GetComponent<InhalerGameGUI>();

        ResetInhalerGame();
    }

    // Initialize the values in InhalerLogic. Then determine whether to show (activate)
    // the Advair inhaler or Rescue inhaler, depending on what InhalerLogic.CurrentInhalerType is
    public void ResetInhalerGame(){
        InhalerLogic.Init(isPracticeGame);

        if (InhalerLogic.CanPlayGame){ // tells us if we can play the game or not (any more plays remaining today)

            if (!introShown){
                inhalerGameGUI.ShowIntro();
                introShown = true;
                float delay;
                if (isPracticeGame){
                    delay = InhalerGameGUI.practiceMessageDuration + InhalerGameGUI.introMessageDuration;
                }
                else {
                    delay = InhalerGameGUI.introMessageDuration;
                }

                Invoke("SetUpScene", delay);
            }
            else {
                SetUpScene();
            }
        }
        gameEnded = false;
    }

    void SetUpScene(){
        DestroyAndRecreatePrefabs();
        SetUpInhalerGame();
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

        if ((InhalerLogic.CurrentInhalerType == InhalerType.Advair && DataManager.FirstTimeAdvair) ||
            (InhalerLogic.CurrentInhalerType == InhalerType.Rescue && DataManager.FirstTimeRescue)){
            runShowHintTimer = false;
            showHint = true;
        }
        else {
            runShowHintTimer = true;
            showHint = false;
            timer = 0;
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
            hasPlayedGame = true;

            // record having given the pet the inhaler, if this was the real game.
            if (!isPracticeGame){
                CalendarLogic.RecordGivingInhaler();
            }
            else {
                DataManager.AddPoints(practiceGamePointIncrement);
            }

            inhalerGameGUI.DisplayMessage();
            RemoveFirstTimeFlags();
            gameEnded = true;
            inhalerGameGUI.HideButtons();
            // Invoke("ShowButtons", inhalerGameGUI.introMessageDuration); // set a 3 second delay so that the "great" message animation has time to play
        }
    }

    void ShowButtons(){
        inhalerGameGUI.ShowButtons();
    }

    void RemoveFirstTimeFlags(){
        if (InhalerLogic.CurrentInhalerType == InhalerType.Advair && DataManager.FirstTimeAdvair){
            DataManager.FirstTimeAdvair = false;
        }
        else if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue && DataManager.FirstTimeRescue){
            DataManager.FirstTimeRescue = false;
        }
    }
}
