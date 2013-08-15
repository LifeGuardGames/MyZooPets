using UnityEngine;
using System.Collections;

public class DiagnoseUIManager : MonoBehaviour {
    public GameObject spritePet;
    public GameObject buttonPanel;
    public GameObject hud;

    private float timer = 0;
    private float timerInterval = 10;
    private bool gameActive = false; //True: game in process, false: game paused or game over
    private AsthmaStage chosenStage; //what stage did the user choose
    private UISlider progressBar; //reference to UI
    private UISprite advair;

	// Use this for initialization
	void Start () {
        timer = timerInterval;
        progressBar = transform.Find("Timer").GetComponent<UISlider>();
        advair = transform.Find("Advair").GetComponent<UISprite>();
        advair.alpha = 0;

        StartGame();	
	}
	
	// Update is called once per frame
	void Update () {
	   if(!gameActive) return;

       timer -= Time.deltaTime;
       progressBar.sliderValue = timer/timerInterval;
       if(timer <= 0){
            gameActive = false;
            HideGUIPanel(false);
            RewardNotification(false);
       }
	}

    //Called when the green, yellow, red buttons are clicked
    public void SymptomsPicked(GameObject buttonClicked){
        switch(buttonClicked.name){
            case "Green": chosenStage = AsthmaStage.OK; break;
            case "Yellow": chosenStage = AsthmaStage.Sick; break;
            case "Red": chosenStage = AsthmaStage.Attack; break;
        }

        if(DiagnoseGameLogic.IsThisStageCorrect(chosenStage)) {
            if(chosenStage.Equals(AsthmaStage.OK)){ //correct answer
                RewardNotification(true);
            }
            HideGUIPanel(true);
        }else{ //wrong answer. go to reward notification right away
            gameActive = false; 
            RewardNotification(false);
            HideGUIPanel(false);
        }
    }

    private void StartGame(){
        DiagnoseGameLogic.Init();

        switch(DiagnoseGameLogic.CurrentStage){
            case AsthmaStage.OK:
                spritePet.GetComponent<tk2dSprite>().SetSprite("OkPet");
            break;
            case AsthmaStage.Sick:
                spritePet.GetComponent<tk2dSprite>().SetSprite("SickPet");
            break;
            case AsthmaStage.Attack:
                spritePet.GetComponent<tk2dSprite>().SetSprite("AttackPet");
            break;
        }
        gameActive = true;
    }

    //Add points and spawn popup
    private void RewardNotification(bool isAnswerCorrect){
        int deltaPoints = 0;
        int deltaStars = 0;

        gameActive = false; //stop the game
        //set points and display notification
        if(isAnswerCorrect){
            deltaPoints = 500;
            deltaStars = 500;
        }else{
            deltaPoints = 100;
            deltaStars = 100;
        }
		
        hud.GetComponent<MoveTweenToggleDemultiplexer>().Show();

        DiagnoseGameLogic.ClaimReward(deltaPoints, deltaStars);

        NotificationUIManager.Instance.EnqueueGameOverRewardMessage(deltaStars, deltaPoints,
            delegate(){
                hud.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
				// TODO-s Call NotificationUIManager.Instance.UnlockQueue???
                Application.LoadLevel("NewBedRoom");
                });
    }


    private void HideGUIPanel(bool isAnswerCorrect){
        if(isAnswerCorrect && (chosenStage.Equals(AsthmaStage.Sick) || chosenStage.Equals(AsthmaStage.Attack))){
            advair.alpha = 1; //show advair. user needs to give advair to pet
        }

        //TO DO: hide gui panel
        buttonPanel.GetComponent<MoveTweenToggle>().Hide();
    }
}
