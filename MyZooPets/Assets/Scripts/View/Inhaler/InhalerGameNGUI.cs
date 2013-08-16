using UnityEngine;
using System;
using System.Collections;

public class InhalerGameNGUI : Singleton<InhalerGameNGUI> {

    public static float practiceMessageDuration = 3.0f; //duration of popup message
    public static float introMessageDuration = 3.0f; //duration of popup message
    public GameObject progressBarObject;
    public GameObject quitButton;
    public GameObject hudObject;
    private ProgressBarController progressBar;

    void Awake(){
        progressBar = progressBarObject.GetComponent<ProgressBarController>();
    }
    void Start(){
        InhalerLogic.OnNextStep += UpdateProgressBar;
    }

    void OnDestroy(){
        InhalerLogic.OnNextStep -= UpdateProgressBar;
    }

    //Event listener. listens to OnNext Step and Fill progress bar by one node
    private void UpdateProgressBar(object sender, EventArgs args){
        progressBar.UpdateStep(InhalerLogic.Instance.CurrentStep - 1);
    }
    public void RestartProgressBar(){
        int numOfNodes = GetNumOfNodes();
        if(numOfNodes < 2){
            Debug.LogError("Number of nodes cannot be less than 2");
        }
        progressBar.Init(numOfNodes);
    }
    public void HideProgressBar(){
        progressBarObject.SetActive(false);
    }
    public void ShowProgressBar(){
        progressBarObject.SetActive(true);
    }
    public void ShowHUD(){
        hudObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
    }
    public void HideHUD(){
        hudObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
    }
    public void ShowQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Show();
    }
    public void HideQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Hide();
    }
    public void ShowGameOverMessage(){
        if (InhalerLogic.Instance.IsPracticeGame){
            NotificationUIManager.Instance.EnqueueGameOverRewardMessage(
                InhalerGameManager.Instance.PracticeGameStarIncrement,
                InhalerGameManager.Instance.PracticeGamePointIncrement,
                delegate (){
                    InhalerGameManager.Instance.ResetInhalerGame();
                    RestartProgressBar();
                },
                QuitInhalerGame
            );
        }
        else {
            NotificationUIManager.Instance.EnqueueGameOverRewardMessage(
                InhalerGameManager.Instance.RealGameStarIncrement,
                InhalerGameManager.Instance.RealGamePointIncrement,
                QuitInhalerGame
            );

        }
    }

    //Display game introduction popup texture
    public void ShowIntro(){
        HideProgressBar();
        float messageDuration;
        if (InhalerLogic.Instance.IsPracticeGame){
            // Note: NotificationUIManager knows to call PopupTexture("intro") after calling PopupTexture("practice intro").
            NotificationUIManager.Instance.PopupTexture("practice intro");
            messageDuration = introMessageDuration + practiceMessageDuration;
        }
        else {
            NotificationUIManager.Instance.PopupTexture("intro");
            messageDuration = introMessageDuration;
        }

        Invoke("ShowProgressBar", messageDuration);
    }

    //Return number of steps in inhaler sequence
    private int GetNumOfNodes(){
        int numSteps = 0;
        if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Advair){
            numSteps = InhalerLogic.ADVAIR_NUM_STEPS;
        }
        else if (InhalerLogic.Instance.CurrentInhalerType == InhalerType.Rescue){
            numSteps = InhalerLogic.RESCUE_NUM_STEPS;
        }
        return numSteps;
    }

    private void QuitInhalerGame(){
		// TODO-s Call notificationUIManager.Instance.UnlockQueue();?????
        Application.LoadLevel("NewBedRoom");
    }
}