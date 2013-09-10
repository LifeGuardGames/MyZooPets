using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerGameUIManager : Singleton<InhalerGameUIManager> {
    public static EventHandler<EventArgs> OnShowHint; //Fire this event when hints need to display 
    public static float introMessageDuration = 3.0f; //duration of popup message

    public GameObject progressBarObject;
    public GameObject quitButton;
    public GameObject progressStep; //Prefabs for thesliderNodes 
    public UISlider slider; //Reference of UISlider

    private List<GameObject> sliderNodes; //list of nodes to show game steps
    private int stepCompleted;
    private float increment; //How much to incement the slider by
    private bool showHint = false; //display swipe hints for the inhaler
    private bool runShowHintTimer = true; //True: start running hint timer
    private float timer = 0; //hint timer
    private float timeBeforeHints = 5.0f; //5 seconds before the hint is shown
    private int pointIncrement = 250;
    private int starIncrement = 0;

    public bool ShowHint{
        get {return showHint;}
    }

    void Awake(){
        slider.sliderValue = 0;
        slider.numberOfSteps = InhalerLogic.RESCUE_NUM_STEPS;
        increment = 1.0f / (slider.numberOfSteps - 1);

        stepCompleted = 0;
        sliderNodes = new List<GameObject>();
        SetUpProgressSteps();
        UpdateNodeColors();
    }

    void Start(){
        InhalerLogic.OnNextStep += UpdateProgressBar;
        InhalerLogic.OnGameOver += OnGameEnd;
        InhalerLogic.OnNextStep += OnNextStep;

        StartGame();
    }

    void OnDestroy(){
        InhalerLogic.OnNextStep -= UpdateProgressBar;
        InhalerLogic.OnGameOver -= OnGameEnd;
        InhalerLogic.OnNextStep -= OnNextStep;
    }

    /*
        If runShowHintTimer is true, hints will be hidden at first, and shown only when the user has not made the correct move
        after a specified period of time (timeBeforeHints).
        If it is false, that means the hints should be shown throughout the game (for someone's first time playing this).
    */
    void Update(){
        if(runShowHintTimer){
            ShowHintTimer(); // This checks and shows hints if necessary.
        }
    }
 
    public void HideProgressBar(){
        progressBarObject.SetActive(false);
    }

    public void ShowProgressBar(){
        progressBarObject.SetActive(true);
    }

    public void ShowHUD(){
        HUDUIManager.Instance.ShowPanel();
    }

    public void HideHUD(){
        HUDUIManager.Instance.HidePanel();
    }

    public void ShowQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Show();
    }

    public void HideQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Hide();
    }


    private void StartGame(){
        HideHUD();
        ShowIntro();
        SetUpHintTimer();
    }

    private void SetUpHintTimer(){
        //Show hint right away if it's users' first time
        if(InhalerLogic.Instance.IsFirstTimeRescue){ 
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
            if(D.Assert(OnShowHint != null, "OnShowHint has no listeners"))
                OnShowHint(this, EventArgs.Empty);
        }
    }

    //Timer is reset every time the current step changes
    private void ResetHintTimer(){
        timer = 0;
        showHint = false; 
    }

    private void QuitInhalerGame(){

		// TODO-s Call notificationUIManager.Instance.UnlockQueue();?????
        // Add scene transition as well
        Application.LoadLevel("NewBedRoom");
    }

    //Event listener. Listens to when user moves on to the next step
    private void OnNextStep(object sender, EventArgs args){
        if(!InhalerLogic.Instance.IsFirstTimeRescue)
            ResetHintTimer();
    }

    //Event listener. Listens to game over message. Show game over popup/clean up game
    private void OnGameEnd(object sender, EventArgs args){
        // Record having given the pet the inhaler, if this was the real game.
        StatsController.Instance.ChangeStats(pointIncrement, Vector3.zero, 
            starIncrement, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);

        ShowGameOverMessage();
        ShowHUD();
        HideQuitButton();
    }

    //=================================Progress Bar=============================
    //Event listener. listens to OnNext Step and Fill progress bar by one node
    private void UpdateProgressBar(object sender, EventArgs args){
        stepCompleted = InhalerLogic.Instance.CurrentStep -1;
        slider.sliderValue = stepCompleted * increment;
        UpdateNodeColors();
    }

    // Set up the sliderNodes, based on how the slider is set up.
    private void SetUpProgressSteps(){
        Transform foreground = slider.transform.Find("Foreground");
        float width = foreground.transform.localScale.x;
        float increment = width / (slider.numberOfSteps - 1);

        for (int i = 0; i < slider.numberOfSteps; i++){
            GameObject node = NGUITools.AddChild(progressBarObject, progressStep);
            node.transform.localPosition = new Vector3(i * increment, 0, 0);

            UILabel label = node.transform.Find("Label").GetComponent<UILabel>();
            label.text = i.ToString();
            sliderNodes.Add(node);
        }
    }


    // Loop through all step sliderNodes, and set their colors accordingly.
    private void UpdateNodeColors(){
        for (int i = 0; i < sliderNodes.Count; i++){
            if (i <= stepCompleted){
                sliderNodes[i].transform.Find("Sprite").GetComponent<UISprite>().spriteName="circleRed";
            }
            else {
                sliderNodes[i].transform.Find("Sprite").GetComponent<UISprite>().spriteName="circleGray";
            }
        }
    }
    //========================================================================

    //Display game introduction popup texture
    private void ShowIntro(){
        HideProgressBar();
        float messageDuration;
   
        NotificationUIManager.Instance.PopupTexture("intro");
        messageDuration = introMessageDuration;

        Invoke("ShowProgressBar", messageDuration);
    }

    private void ShowGameOverMessage(){
        // Assign delegate functions to be passed in hashtable
        PopupNotificationNGUI.HashEntry button1Function = delegate(){
                QuitInhalerGame();
            };
        
        // Populate notification entry table
        Hashtable notificationEntry = new Hashtable();
        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GameOverRewardOneButton);
        notificationEntry.Add(NotificationPopupFields.DeltaStars, starIncrement);
        notificationEntry.Add(NotificationPopupFields.DeltaPoints, pointIncrement);
        notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);

        // Place notification entry table in static queue
        NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }
}