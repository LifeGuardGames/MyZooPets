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
    private ProgressBarController progressBar;

    void Start(){
        notificationUIManager = GameObject.Find("NotificationUIManager").GetComponent<NotificationUIManager>();
        progressBar = progressBarObject.GetComponent<ProgressBarController>();

        RestartProgressBar();
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
        progressBarObject.active = false;
    }
    public void ShowProgressBar(){
        progressBarObject.active = true;
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
                CalendarLogic.StarIncrement,
                CalendarLogic.PointIncrement,
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
        if (inhalerGameManager.isPracticeGame){
            notificationUIManager.PopupTexture("practice intro");
        }
        else {
            notificationUIManager.PopupTexture("intro");
        }

        Invoke("ShowIntroEnd", introMessageDuration);
    }

    void ShowIntroEnd(){
        ShowProgressBar();
    }

    void QuitInhalerGame(){
        RestartProgressBar();
        Application.LoadLevel("NewBedRoom");
    }
}