using UnityEngine;
using System.Collections;

public class DiagnoseLogic : MonoBehaviour {

    private NotificationUIManager notificationUIManager;
    public AsthmaStage CurrentStage{get; set;}

    //check if the stage chosen by the user is correct
    public bool IsThisStageCorrect(AsthmaStage asthmaStage){
        return asthmaStage.Equals(CurrentStage);
    }

	// Use this for initialization
	void Start () {
	   notificationUIManager = GameObject.Find("Main Camera/NotificationUIManager").GetComponent<NotificationUIManager>();
       Test();
       Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void Test(){
        notificationUIManager.PopupNotification("sup", 
            delegate(){
                print("wefiowefjwfe");
            },
            delegate(){
                print("sup");
            }
        );
    }

    private void GenerateRandomStage(){
        int randomNumber = Random.Range(0, 3);
        switch(randomNumber){
            case 0: CurrentStage = AsthmaStage.Discomfort; break;
            case 1: CurrentStage = AsthmaStage.Sick; break;
            case 2: CurrentStage = AsthmaStage.Attack; break;
        }
    }

}
