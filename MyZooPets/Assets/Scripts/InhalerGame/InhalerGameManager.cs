using UnityEngine;
using System;
using System.Collections;

public class InhalerGameManager : Singleton<InhalerGameManager>{

    public GameObject advairPrefab;
    public GameObject rescuePrefab;
    public GameObject rescueShakerPrefab; 	// Arrows that indicate that the rescue inhaler has to be shaken
    public GameObject inhaleExhalePrefab; 	// Arrows that indicate whether to breathe in or out
    public GameObject smallRescuePrefab; 	// Rescue inhaler that appears in front of the pet's mouth

    private GameObject advair;
    private GameObject rescue;
    private GameObject rescueShaker; 		// Arrows that indicate that the rescue inhaler has to be shaken
    private GameObject inhaleExhale; 		// Arrows that indicate whether to breathe in or out
    private GameObject smallRescue; 		// Rescue inhaler that appears in front of the pet's mouth

	public GameObject textPrefabShake;		// Attach text to the hints that popup
	public GameObject textPrefabSwipe;
	public GameObject textPrefabDrag;

    private int practiceGamePointIncrement = 50;
    private int practiceGameStarIncrement = 50;
    private int realGamePointIncrement = 250;
    private int realGameStarIncrement = 0;
    private bool showHint = false; //display swipe hints for the inhaler
    private bool runShowHintTimer = true;
    private float timer = 0; //hint timer
    private float timeBeforeHints = 5.0f; //5 seconds before the hint is shown
    private bool introShown = false; //has the intro text been shown

    public int PracticeGamePointIncrement {
        get {return practiceGamePointIncrement;}
    }
    public int PracticeGameStarIncrement{
        get{return practiceGameStarIncrement;}
    }
    public int RealGamePointIncrement {
        get {return realGamePointIncrement;}
    }
    public int RealGameStarIncrement{
        get{return realGameStarIncrement;}
    }
    public bool ShowHint{
        get {return showHint;}
    }

    void Start(){
        InhalerLogic.OnGameOver += OnGameEnd;
        ResetInhalerGame();
    }

     /*
        If runShowHintTimer is true, hints will be hidden at first, and shown only when the user has not made the correct move
        after a specified period of time (timeBeforeHints).
        If it is false, that means the hints should be shown throughout the game (for someone's first time playing this).
    */
    void Update(){
        if (runShowHintTimer){
            ShowHintTimer(); // This checks and shows hints if necessary.
        }
    }

    void OnDestroy(){
        InhalerLogic.OnGameOver -= OnGameEnd;
    }

    // Initialize the values in InhalerLogic. Then determine whether to show (activate)
    // the Advair inhaler or Rescue inhaler, depending on what InhalerLogic.CurrentInhalerType is
    public void ResetInhalerGame(){
        InhalerGameNGUI.Instance.HideHUD();
        InhalerLogic.Instance.ResetGame();

        if (!introShown){ // Shows "Use The Inhaler" message if playing the first round of the game
            InhalerGameNGUI.Instance.RestartProgressBar();
            InhalerGameNGUI.Instance.ShowIntro();
            introShown = true;

            // Calculate how long to wait before creating the inhalers.
            float delay;
            if (InhalerLogic.Instance.IsPracticeGame){
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

    private void SetUpScene(){
        DestroyAndRecreatePrefabs(); // Because adding fresh copies is easier than going back and fixing modified values.
        SetUpInhalerGame();
        InhalerGameNGUI.Instance.ShowQuitButton();
    }

    // This creates both the advair and rescue inhalers. Hiding of the unnecessary ones is done in SetUpInhalerGame().
    private void DestroyAndRecreatePrefabs(){
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
    private void SetUpInhalerGame(){
        // Debug.Log("Current inhaler type is -> " + InhalerLogic.CurrentInhalerType);
        if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair){
            rescue.SetActive(false);
            rescueShaker.SetActive(false);
        }
        else if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            advair.SetActive(false);
        }
        smallRescue.SetActive(false);

        //Show hint right away if it's users' first time
        if ((InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair && DataManager.Instance.Inhaler.FirstTimeAdvair) ||
            (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue && DataManager.Instance.Inhaler.FirstTimeRescue)){
            runShowHintTimer = false;
            showHint = true;
        }
        else {
            runShowHintTimer = true;
            showHint = false;
            timer = 0;
        }
    }

    /*
        Hints will be hidden at first, and shown only when the user has not made the correct move after a specified
        period of time (timeBeforeHints).
    */
    private void ShowHintTimer(){ // to be called in Update()
        timer += Time.deltaTime;
        if (timer > timeBeforeHints){
            showHint = true;
        }
    }

    //Timer is reset every time the current step changes
    private void ResetHintTimer(){
        timer = 0;
        showHint = false; 
    }

     private void RemoveFirstTimeFlags(){
        if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair && DataManager.Instance.Inhaler.FirstTimeAdvair){
            DataManager.Instance.Inhaler.FirstTimeAdvair = false;
        }
        else if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue && DataManager.Instance.Inhaler.FirstTimeRescue){
            DataManager.Instance.Inhaler.FirstTimeRescue = false;
        }
    }

    //Event listener. Listens to when user moves on to the next step
    private void OnNextStep(object sender, EventArgs args){
        ResetHintTimer();
    }

    //Event listener.
    private void OnGameEnd(object sender, EventArgs args){
        // Record having given the pet the inhaler, if this was the real game.
        if (!InhalerLogic.Instance.IsPracticeGame){
            CalendarLogic.RecordGivingInhaler();
            StatsController.Instance.ChangeStats(realGamePointIncrement, Vector3.zero, 
                realGameStarIncrement, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
        }
        else {
			StatsController.Instance.ChangeStats(practiceGamePointIncrement, Vector3.zero, 
                practiceGameStarIncrement, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
        }

        InhalerGameNGUI.Instance.ShowGameOverMessage();
        InhalerGameNGUI.Instance.ShowHUD();
        InhalerGameNGUI.Instance.HideQuitButton();

        RemoveFirstTimeFlags();
    }
}
