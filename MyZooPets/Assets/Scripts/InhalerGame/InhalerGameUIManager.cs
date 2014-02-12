using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InhalerGameUIManager : Singleton<InhalerGameUIManager> {
    public static EventHandler<EventArgs> OnShowHint; //Fire this event when hints need to display 

    public GameObject progressBarObject;
    public GameObject quitButton;
    public GameObject inhalerBody;
    public SceneTransition scriptTransition; 
    public GetFireAnimationController fireAnimationController; //The script that plays the fire animation at the end of the inhaler game
    public bool tutOn; //turn tutorial on or off. for debuggin

    private bool showHint = false; //display swipe hints for the inhaler
    private bool runShowHintTimer = true; //True: start running hint timer
    private float timer = 0; //hint timer
    private float timeBeforeHints = 5.0f; //5 seconds before the hint is shown
    private int starIncrement = 0;

    public bool ShowHint{
        get {return showHint;}
    }

    void Start(){
        Input.multiTouchEnabled = true;
        InhalerLogic.OnGameOver += OnGameEnd;
        InhalerLogic.OnNextStep += OnNextStep;
        GetFireAnimationController.OnGetFireAnimationDone += OnGetFireAnimationDone;

        StartCoroutine(StartGame());
    }

    void OnDestroy(){
        InhalerLogic.OnGameOver -= OnGameEnd;
        InhalerLogic.OnNextStep -= OnNextStep;
        GetFireAnimationController.OnGetFireAnimationDone -= OnGetFireAnimationDone;
    }

    //---------------------------------------------
    // If runShowHintTimer is true, hints will be hidden at first, 
    // and shown only when the user has not made the correct move
    // after a specified period of time (timeBeforeHints).
    // If it is false, that means the hints should be shown 
    // throughout the game (for someone's first time playing this).
    //----------------------------------------------
    void Update(){
        if(runShowHintTimer){
            ShowHintTimer(); // This checks and shows hints if necessary.
        }
    }
 
    private  void HideProgressBar(){
        progressBarObject.SetActive(false);
    }

    private void ShowProgressBar(){
        progressBarObject.SetActive(true);
    }

    private void ShowGameUI(){
        ShowProgressBar();
        if(OnShowHint != null)
            OnShowHint(this, EventArgs.Empty);
    }

    private void ShowHUD(){
        HUDUIManager.Instance.ShowPanel();
    }

    private void HideHUD(){
        HUDUIManager.Instance.HidePanel();
    }

    private void ShowQuitButton(){
        if(InhalerLogic.Instance.IsTutorialCompleted || !tutOn)
            quitButton.GetComponent<PositionTweenToggle>().Show();
    }

    private void HideQuitButton(){
        if(InhalerLogic.Instance.IsTutorialCompleted || !tutOn)
            quitButton.GetComponent<PositionTweenToggle>().Hide();
    }

    private void HideInhaler(){
        inhalerBody.SetActive(false); 
    }

    private void ShowInhaler(){
        inhalerBody.SetActive(true);
    }

    private IEnumerator StartGame(){
        yield return 0;

        HideHUD();
        ShowQuitButton();
        SetUpHintTimer();

        // Analytics.Instance.StartPlayTimeTracker();

        //Start the first hint
        if(OnShowHint != null)
            OnShowHint(this, EventArgs.Empty);
    }

    //----------------------------------------------------------
    // SetUpHintTimer()
    // Decides whether hints should be display right away or wait
    // for the hint count down timer
    //----------------------------------------------------------
    private void SetUpHintTimer(){
        //Show hint right away if it's users' first time
        if(InhalerLogic.Instance.IsFirstTimeRescue && tutOn){ 
            runShowHintTimer = false;
            showHint = true;
        }else{
            runShowHintTimer = true;
            showHint = false;
            timer = 0;
        }
    }

    //----------------------------------------------------------
    // ShowHintTimer()
    //  Hints will be hidden at first, and shown only when the user 
    // has not made the correct move after a specified period of time (timeBeforeHints).
    //----------------------------------------------------------
    private void ShowHintTimer(){ // to be called in Update()
        timer += Time.deltaTime;
        if (timer > timeBeforeHints){
            showHint = true;

            Analytics.Instance.InhalerHintRequired(InhalerLogic.Instance.CurrentStep);

            if(OnShowHint != null)
                OnShowHint(this, EventArgs.Empty);

            ResetHintTimer();
        }
    }

    //----------------------------------------------------------
    // ResetHintTimer()
    //Timer is reset every time the current step changes
    //----------------------------------------------------------
    private void ResetHintTimer(){
        timer = 0;
        showHint = false; 
    }

    private void QuitInhalerGame(){
        InhalerLogic.Instance.CompleteTutorial();
        NotificationUIManager.Instance.CleanupNotification();
        scriptTransition.StartTransition(SceneUtils.BEDROOM);
    }

    //Event listener. Listens to when user moves on to the next step
    private void OnNextStep(object sender, EventArgs args){
        if(!InhalerLogic.Instance.IsFirstTimeRescue || !tutOn)
            ResetHintTimer();

        if(OnShowHint != null)
            OnShowHint(this, EventArgs.Empty);
    }

    //Event listener. Listens to game over message. Play fire animation 
    private void OnGameEnd(object sender, EventArgs args){
        ShowHUD();
        HideQuitButton();
        HideInhaler();
        HideProgressBar();

        //Spawn floaty
        Hashtable option = new Hashtable();
        option.Add("parent", GameObject.Find("Anchor-Center"));
        option.Add("text", Localization.Localize("INHALER_FLOATY_HOLD_BREATH"));
        option.Add("textSize", 100);
        option.Add("color", Color.white);

        FloatyUtil.SpawnFloatyText(option);

        //play sound
        AudioManager.Instance.PlayClip("inhalerFireFlow");

        //play animation
        fireAnimationController.PlaySequence();
    }

    //Event listener. continue the game after GetFireAnimation is done
    private void OnGetFireAnimationDone(object sender, EventArgs args){
        StatsController.Instance.ChangeFireBreaths(1);
        AudioManager.Instance.PlayClip("inhalerShiningFireIcon");
        
        Invoke("GiveReward", 1.0f);
    }

    //Reward player after the animation is done
    private void GiveReward(){
		int nXP = DataLoader_XpRewards.GetXP( "DailyInhaler", new Hashtable() );
        StatsController.Instance.ChangeStats(nXP, Vector3.zero, 
            starIncrement, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, false, false, false);
        Invoke("QuitInhalerGame", 2.0f);
        // ShowGameOverMessage();    
    }

    // private void ShowGameOverMessage(){
    //     // Assign delegate functions to be passed in hashtable
    //     PopupNotificationNGUI.HashEntry button1Function = delegate(){
    //             QuitInhalerGame();
    //         };
        
    //     // Populate notification entry table
    //     Hashtable notificationEntry = new Hashtable();
    //     notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GameOverRewardOneButton);
    //     notificationEntry.Add(NotificationPopupFields.DeltaStars, starIncrement);
    //     notificationEntry.Add(NotificationPopupFields.DeltaPoints, pointIncrement);
    //     notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);

    //     // Place notification entry table in static queue
    //     NotificationUIManager.Instance.AddToQueue(notificationEntry);
    // }
}