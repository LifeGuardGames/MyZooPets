using UnityEngine;
using System.Collections;

public class InhalerGameNGUI : MonoBehaviour {

    public static float practiceMessageDuration = 3.0f;
    public static float introMessageDuration = 3.0f;

    public float speed;

    private int currentNode;

    private NotificationUIManager notificationUIManager;

    public InhalerGameManager inhalerGameManager;
    public GameObject progressBarObject;
    public GameObject quitButton;
    private ProgressBarController progressBar;

    void Start(){
        notificationUIManager = GameObject.Find("NotificationUIManager").GetComponent<NotificationUIManager>();
        progressBar = progressBarObject.GetComponent<ProgressBarController>();
    }

    public void RestartProgressBar(){
        currentNode = 0;
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

    public void ShowGameOverMessage(){
        if (inhalerGameManager.isPracticeGame){
            notificationUIManager.GameOverRewardMessage(
                inhalerGameManager.PracticeGameStarIncrement,
                inhalerGameManager.PracticeGamePointIncrement,
                delegate (){
                    inhalerGameManager.ResetInhalerGame();
                    RestartProgressBar();
                    // ShowButtons();
                },
                QuitInhalerGame
            );
        }
        else {
            notificationUIManager.GameOverRewardMessage(
                250,
                0,
                QuitInhalerGame
            );

        }
    }

    int GetNumOfNodes(){
        int numSteps = 0;
        if (InhalerLogic.CurrentInhalerType == InhalerType.Advair){
            numSteps = 5;
        }
        else if (InhalerLogic.CurrentInhalerType == InhalerType.Rescue){
            numSteps = 6;
        }
        return numSteps + 1;
    }

    void Update(){
        int lastCompletedStep = InhalerLogic.CurrentStep - 1;
        if(currentNode != lastCompletedStep){
            currentNode = lastCompletedStep;
            progressBar.UpdateStep(lastCompletedStep);
        }

    }

    public void ShowIntro(){
        HideProgressBar();
        float messageDuration;
        if (inhalerGameManager.isPracticeGame){
            // Note: NotificationUIManager knows to call PopupTexture("intro") after calling PopupTexture("practice intro").
            notificationUIManager.PopupTexture("practice intro");
            messageDuration = introMessageDuration + practiceMessageDuration;
        }
        else {
            notificationUIManager.PopupTexture("intro");
            messageDuration = introMessageDuration;
        }

        Invoke("ShowIntroEnd", messageDuration);
    }

    void ShowIntroEnd(){
        ShowProgressBar();
    }

    public void ShowQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Show();
    }
    public void HideQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Hide();
    }

    void QuitInhalerGame(){
        Application.LoadLevel("NewBedRoom");
    }
}