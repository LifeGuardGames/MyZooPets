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

    public bool isPracticeGame; // Is this a practice game, or the real thing (which counts towards the calendar tally)

    int practiceGamePointIncrement = 50;
    int practiceGameStarIncrement = 50;
    public int PracticeGamePointIncrement {
        get {return practiceGamePointIncrement;}
    }
    public int PracticeGameStarIncrement{
		get{return practiceGameStarIncrement;}
	}

    InhalerGameNGUI inhalerGameNGUI;

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
        inhalerGameNGUI = GameObject.Find("InhalerGameNGUI").GetComponent<InhalerGameNGUI>();

        ResetInhalerGame();
    }

    // Initialize the values in InhalerLogic. Then determine whether to show (activate)
    // the Advair inhaler or Rescue inhaler, depending on what InhalerLogic.CurrentInhalerType is
    public void ResetInhalerGame(){
        InhalerLogic.Init(isPracticeGame);
        inhalerGameNGUI.HideHUD();

        if (InhalerLogic.CanPlayGame){ // tells us if we can play the game or not (any more plays remaining today)

            if (!introShown){ // Shows "Use The Inhaler" message if playing the first round of the game
                inhalerGameNGUI.RestartProgressBar();
                inhalerGameNGUI.ShowIntro();
                introShown = true;

                // Calculate how long to wait before creating the inhalers.
                float delay;
                if (isPracticeGame){
                    delay = InhalerGameNGUI.practiceMessageDuration + InhalerGameNGUI.introMessageDuration;
                }
                else {
                    delay = InhalerGameNGUI.introMessageDuration;
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
        DestroyAndRecreatePrefabs(); // Because adding fresh copies is easier than going back and fixing modified values.
        SetUpInhalerGame();
        inhalerGameNGUI.ShowQuitButton();
    }

    // This creates both the advair and rescue inhalers. Hiding of the unnecessary ones is done in SetUpInhalerGame().
    void DestroyAndRecreatePrefabs(){
        // delete existing gameobjects from the last round if there are any
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

    /*
        Disable (hide) the game components that aren't required for this round.
        Eg. Hide all rescue inhaler components if InhalerLogic.CurrentInhalerType is InhalerType.Advair.
    */
    void SetUpInhalerGame(){
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

        /*
            If runShowHintTimer is true, hints will be hidden at first, and shown only when the user has not made the correct move
            after a specified period of time (timeBeforeHints).
            If it is false, that means the hints should be shown throughout the game (for someone's first time playing this).
        */

        if (runShowHintTimer){
            ShowHintTimer(); // This checks and shows hints if necessary.
        }
    }

    /*
        Hints will be hidden at first, and shown only when the user has not made the correct move after a specified
        period of time (timeBeforeHints).
        The timer is reset every time the current step changes.
    */

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

            // Record having given the pet the inhaler, if this was the real game.
            if (!isPracticeGame){
                CalendarLogic.RecordGivingInhaler();
            }
            else {
				StatsController.Instance.ChangeStats(practiceGamePointIncrement, practiceGameStarIncrement, 0, 0, Vector3.zero);
            }

            inhalerGameNGUI.ShowGameOverMessage();
            inhalerGameNGUI.ShowHUD();
            inhalerGameNGUI.HideQuitButton();

            RemoveFirstTimeFlags();
            gameEnded = true;
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
}
