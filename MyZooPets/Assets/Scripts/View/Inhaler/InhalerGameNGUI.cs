using UnityEngine;
using System.Collections;

public class InhalerGameNGUI : MonoBehaviour {

    public static float practiceMessageDuration = 3.0f;
    public static float introMessageDuration = 3.0f;

    public float speed;

    private int currentNode;

    public InhalerGameManager inhalerGameManager;
    public GameObject progressBarObject;
    public GameObject quitButton;
    private ProgressBarController progressBar;
    public GameObject hudObject;

    void Start(){
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

    public void ShowHUD(){
        hudObject.GetComponent<MoveTweenToggleDemultiplexer>().Show();
    }
    public void HideHUD(){
        hudObject.GetComponent<MoveTweenToggleDemultiplexer>().Hide();
    }

    public void ShowGameOverMessage(){
        if (inhalerGameManager.isPracticeGame){
            NotificationUIManager.Instance.EnqueueGameOverRewardMessage(
                inhalerGameManager.PracticeGameStarIncrement,
                inhalerGameManager.PracticeGamePointIncrement,
                delegate (){
                    inhalerGameManager.ResetInhalerGame();
                    RestartProgressBar();
                },
                QuitInhalerGame
            );
        }
        else {
            NotificationUIManager.Instance.EnqueueGameOverRewardMessage(
                inhalerGameManager.RealGameStarIncrement,
                inhalerGameManager.RealGamePointIncrement,
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
            NotificationUIManager.Instance.PopupTexture("practice intro");
            messageDuration = introMessageDuration + practiceMessageDuration;
        }
        else {
            NotificationUIManager.Instance.PopupTexture("intro");
            messageDuration = introMessageDuration;
        }

        Invoke("ShowProgressBar", messageDuration);
    }

    public void ShowQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Show();
    }
    public void HideQuitButton(){
        quitButton.GetComponent<MoveTweenToggle>().Hide();
    }

    void QuitInhalerGame(){
		// TODO-s Call notificationUIManager.Instance.UnlockQueue();?????
        Application.LoadLevel("NewBedRoom");
    }
}