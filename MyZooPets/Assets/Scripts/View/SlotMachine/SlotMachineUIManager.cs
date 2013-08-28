using UnityEngine;
using System.Collections;

public class SlotMachineUIManager : MonoBehaviour {
    public GameObject slotMachine;
    private Transform[] wheels = new Transform[3]; //reference to the 3 wheels inside slot machine
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    private int costStars = 50; //costs 50 stars to play

    void Start(){
        int counter = 0;

        foreach(Transform wheel in slotMachine.transform){ //obtain reference to 3 spinning wheels
            wheels[counter] = wheel;
            counter++;
        }

        if (DataManager.Instance.Stats.Stars >= costStars){
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
	                StatsController.Instance.ChangeStats(0, Vector3.zero, costStars * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
                    StartGame();
	            };
			PopupNotificationNGUI.HashEntry button2Function = delegate() {
					Application.LoadLevel("NewBedRoom");
				};
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TwoButtons);
			notificationEntry.Add(NotificationPopupFields.Message, "-"+ costStars +" stars to play");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
			notificationEntry.Add(NotificationPopupFields.Button1Label, "Start");
			notificationEntry.Add(NotificationPopupFields.Button2Label, "Back");
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
			
//            NotificationUIManager.Instance.EnqueuePopupNotificationTwoButtons(
//                "-"+ costStars +" stars to play",
//                delegate(){
//                    StatsController.Instance.ChangeStats(0, Vector3.zero, costStars * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
//                    StartGame();
//                },
//                delegate(){
//                    Application.LoadLevel("NewBedRoom");
//                },
//                "Start",
//                "Back");
        }
        else { // not enough stars to play
			
			/////// Send Notication ////////
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){
	                Application.LoadLevel("NewBedRoom");
	            };
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.OneButton);
			notificationEntry.Add(NotificationPopupFields.Message, "You need at least " + costStars + " stars to play!");
			notificationEntry.Add(NotificationPopupFields.Button1Label, "Back");
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
			
//            NotificationUIManager.Instance.EnqueuePopupNotificationOneButton(
//                "You need at least " + costStars + " stars to play!",
//                delegate(){
//                    Application.LoadLevel("NewBedRoom");
//                },
//                "Back");

        }

    }

	// Update is called once per frame
	void Update () {
        if(!SlotMachineLogic.GameOver){ //keep checking if wheels have been spinned if game is not over
            if(wheels[0].GetComponent<SpinningWheel>().doneSpinning &&
                wheels[1].GetComponent<SpinningWheel>().doneSpinning &&
                wheels[2].GetComponent<SpinningWheel>().doneSpinning){
                if(SlotMachineLogic.SpinEndCallBack != null) SlotMachineLogic.SpinEndCallBack();

                SlotMachineLogic.GameOver = true; //stop update from checking
                Invoke("Reward", 0.5f);

                //reset wheels to original state so user can spin again right aways
                wheels[0].GetComponent<SpinningWheel>().doneSpinning = false;
                wheels[1].GetComponent<SpinningWheel>().doneSpinning = false;
                wheels[2].GetComponent<SpinningWheel>().doneSpinning = false;
            }
        }
	}

    private void Reward(){
        int stars;
        int points;
        if(SlotMachineLogic.CheckMatch()){
            stars = 500;
            points = 100;
        }else{
            stars = 200;
            points = 10;
        }
        
		StatsController.Instance.ChangeStats(points, Vector3.zero, stars, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
		
		/////// Send Notication ////////
		// Assign delegate functions to be passed in hashtable
		PopupNotificationNGUI.HashEntry button1Function = delegate(){
                StartGame();
            };
		PopupNotificationNGUI.HashEntry button2Function = delegate() {
				Application.LoadLevel("NewBedRoom");
			};
		
		// Populate notification entry table
		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GameOverRewardTwoButton);
		notificationEntry.Add(NotificationPopupFields.DeltaStars, stars);
		notificationEntry.Add(NotificationPopupFields.DeltaPoints, points);
		notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
		
		// Place notification entry table in static queue
		NotificationUIManager.Instance.AddToQueue(notificationEntry);
		
//        NotificationUIManager.Instance.EnqueueGameOverRewardMessage(stars, points,
//            delegate(){
//                StartGame();
//            },
//            delegate(){
//                Application.LoadLevel("NewBedRoom");
//            });
		
		////////////////////////////////
    }

    //start spinning the wheels
    private void StartGame(){
        SlotMachineLogic.GameOver = false;
        SlotMachineLogic.GenerateRandomSlots();
        for(int i = 0; i<3; i++){
            wheels[i].GetComponent<SpinningWheel>().StartSpin(SlotMachineLogic.ChosenSlots[i], i);
        }
    }
}
