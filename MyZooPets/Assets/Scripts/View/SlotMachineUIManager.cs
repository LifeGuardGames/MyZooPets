using UnityEngine;
using System.Collections;

public class SlotMachineUIManager : MonoBehaviour {
    public GameObject slotMachine;
    public NotificationUIManager notificationUIManager; //reference
    private Transform[] wheels = new Transform[3]; //reference to the 3 wheels inside slot machine
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    void Awake(){
        int counter = 0;

        foreach(Transform wheel in slotMachine.transform){ //obtain reference to 3 spinning wheels
            wheels[counter] = wheel;
            counter++;
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

                if(SlotMachineLogic.CheckMatch()){ //check if the user won
                    notificationUIManager.PopupTexture("great");
                }else{
                    notificationUIManager.PopupTexture("nice try");
                }

                //reset wheels to original state so user can spin again right aways
                wheels[0].GetComponent<SpinningWheel>().doneSpinning = false;
                wheels[1].GetComponent<SpinningWheel>().doneSpinning = false;
                wheels[2].GetComponent<SpinningWheel>().doneSpinning = false;
            }
        }
	}

    void OnGUI(){
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }

        if(GUI.Button(new Rect(NATIVE_WIDTH-100, 100, 100, 100), "Quit")){
            Application.LoadLevel("NewBedRoom");
        }

        if(GUI.Button(new Rect(NATIVE_WIDTH/2 - 50, NATIVE_HEIGHT-100, 100, 100), "Spin!")){
            StartGame();
        }
    }

    //start spinning the wheels
    private void StartGame(){
        SlotMachineLogic.GameOver = false;
        for(int i = 0; i<3; i++){
            SlotMachineLogic.GenerateRandomSlots();
            wheels[i].GetComponent<SpinningWheel>().StartSpin(SlotMachineLogic.ChosenSlots[i], i);    
        }
    }

   

    
}
