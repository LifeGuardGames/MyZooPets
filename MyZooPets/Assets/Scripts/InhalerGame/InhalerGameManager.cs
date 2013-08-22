using UnityEngine;
using System;
using System.Collections;

public class InhalerGameManager : Singleton<InhalerGameManager>{

    public GameObject advairPrefab; 
    public GameObject rescuePrefab;
    public GameObject inhale; //Game object that contains hint controller for inhale step
    public GameObject exhale; //contains hint controller for exhale step
    public GameObject drag; //contains hint controller for drag step
    private GameObject advair; //Game object that contains all the parts for advair
    private GameObject rescue; //Game object that contains all the parts for rescue inhaler	

    private int practiceGamePointIncrement = 50;
    private int practiceGameStarIncrement = 50;
    private int realGamePointIncrement = 250;
    private int realGameStarIncrement = 0;

    private bool showHint = false; //display swipe hints for the inhaler
    private bool runShowHintTimer = true; //True: start running hint timer
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

        if(!introShown){ // Shows "Use The Inhaler" message if playing the first round of the game
            InhalerGameNGUI.Instance.RestartProgressBar();
            InhalerGameNGUI.Instance.ShowIntro();
            introShown = true;

            // Calculate how long to wait before creating the inhalers.
            float delay;
            if(InhalerLogic.Instance.IsPracticeGame){
                delay = InhalerGameNGUI.practiceMessageDuration + InhalerGameNGUI.introMessageDuration;
            }
            else{
                delay = InhalerGameNGUI.introMessageDuration;
            }

            Invoke("SetUpScene", delay);
        }
        else{
            SetUpScene();
        }
    }

    private void SetUpScene(){
        DestroyAndRecreatePrefabs(); // Because adding fresh copies is easier than going back and fixing modified values.
        SetUpInhalerGame();
        InhalerGameNGUI.Instance.ShowQuitButton();
    }

    // This destories and creates an advair or a rescue inhaler.
    private void DestroyAndRecreatePrefabs(){
        if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair){
            // delete existing gameobjects from the last round if there are any
            Destroy(advair);
            // instantiate new prefabs and store references to new gameobjects
            advair = Instantiate(advairPrefab) as GameObject;
            advair.name = advairPrefab.name;
        }else if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            Destroy(rescue);
            rescue = Instantiate(rescuePrefab) as GameObject;
            rescue.name = rescuePrefab.name;
        }
    }

    /*
        Determine if HintTimer should be started right away
        Make sure shared actions like inhale, exhale, drag have the correct stepID
    */
    private void SetUpInhalerGame(){
        HintController inhaleHintController = inhale.GetComponent<HintController>();
        HintController exhaleHintController = exhale.GetComponent<HintController>();
        HintController dragHintController = drag.GetComponent<HintController>();

        if(InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            inhaleHintController.inhalerType = InhalerType.Rescue;
            inhaleHintController.showOnStep = 6;
            exhaleHintController.inhalerType = InhalerType.Rescue;
            exhaleHintController.showOnStep = 3;
            dragHintController.inhalerType = InhalerType.Rescue;
            dragHintController.showOnStep = 4;
        }else{
            inhaleHintController.inhalerType = InhalerType.Advair;
            inhaleHintController.showOnStep = 5;
            exhaleHintController.inhalerType = InhalerType.Advair;
            exhaleHintController.showOnStep = 3;
            dragHintController.inhalerType = InhalerType.Advair;
            dragHintController.showOnStep = 4;
        }

        //Show hint right away if it's users' first time
        if((InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair && InhalerLogic.Instance.IsFirstTimeAdvair) ||
            (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue && InhalerLogic.Instance.IsFirstTimeRescue)){ 
            runShowHintTimer = false;
            showHint = true;
        }else{
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

    //Remove first time flag so hints won't be shown automatically
    private void RemoveFirstTimeFlags(){
        if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair && 
            InhalerLogic.Instance.IsFirstTimeAdvair){
            InhalerLogic.Instance.IsFirstTimeAdvair = false;
        }else if(InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue && 
            InhalerLogic.Instance.IsFirstTimeRescue){ 
            InhalerLogic.Instance.IsFirstTimeRescue = false;
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
