using UnityEngine;
using System.Collections;

public class SlotMachineUIManager : MonoBehaviour {
    public GameObject slotMachine;
    public NotificationUIManager notificationUIManager; //reference
    private Transform[] wheels = new Transform[3]; //reference to the 3 wheels inside slot machine
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    void Start(){
        int counter = 0;

        foreach(Transform wheel in slotMachine.transform){ //obtain reference to 3 spinning wheels
            wheels[counter] = wheel;
            counter++;
        }

        notificationUIManager.PopupNotificationTwoButtons(
            "-50 stars to play",
            delegate(){
                DataManager.SubtractStars(50);
                StartGame();
            },
            delegate(){
                Application.LoadLevel("NewBedRoom");
            },
            "Start",
            "Back");
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
        DataManager.AddStars(stars);
        DataManager.AddPoints(points);
        notificationUIManager.GameOverRewardMessage(stars, points,
            delegate(){
                StartGame();
            },
            delegate(){
                Application.LoadLevel("NewBedRoom");
            });
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
