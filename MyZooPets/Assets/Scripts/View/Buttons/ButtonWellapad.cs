using System;
using UnityEngine;

//---------------------------------------------------
// ButtonWellapad
// Button that opens the Wellapad.
//---------------------------------------------------
public class ButtonWellapad : LgWorldButton {
	private bool tutDone = false;

	void Start(){
		// for debug/testing -- we may have the wellapad disabled
		bool isWellapadOn = Constants.GetConstant<bool>("WellapadOn");
		if(!isWellapadOn) {
			Debug.LogWarning("NGUI REMOVE CHANGED - CORRECT CODE HERE");
			//NGUITools.SetActive(gameObject, false);
		}

		tutDone = DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished();
		if(tutDone){
			//Listens to update event from wellapad mission controller
			//WellapadMissionController.Instance.OnTaskUpdated += EnableButtonBounce;
			//WellapadMissionController.Instance.OnMissionsRefreshed += EnableButtonBounce;

			//Start bouncing if there are active tasks
			if(WellapadMissionController.Instance.HasActiveTasks())
				EnableButtonBounce(this, EventArgs.Empty);
		}
	}

	protected override void ProcessClick() {
		// Call from fire crystal ui manager > opens wellapad uimanager > opens fire crystal ui
		if(FireCrystalUIManager.Instance.IsOpen())
			FireCrystalUIManager.Instance.CloseUIBasedOnScene();
		else{
			FireCrystalUIManager.Instance.OpenUIBasedOnScene();
			DisableButtonBounce();
		}
	}

	//---------------------------------------------------
	// SetListenersToWellapadMissionController()
	// Listens to when tasks are completed or the mission 
	// has been refreshed
	//---------------------------------------------------
	public void SetListenersToWellapadMissionController(){
		WellapadMissionController.Instance.OnTaskUpdated += EnableButtonBounce;
		WellapadMissionController.Instance.OnMissionsRefreshed += EnableButtonBounce;
		EnableButtonBounce(this, EventArgs.Empty);
	}

	private void EnableButtonBounce(object sender, EventArgs args){
//		AnimationControl animControl = GetComponent<AnimationControl>(); 	// TODO Disable this for now
//		if(animControl != null){
//			animControl.Play("smallBounceHard");
//		}
//		if(sunBeam){
//			sunBeam.SetActive(true);
//		}
	}

	private void DisableButtonBounce(){
//		AnimationControl animControl = GetComponent<AnimationControl>();
//		if(animControl.IsPlaying("smallBounceHard")){
//			animControl.StopAndResetFrame("zeroPointEight");
//		}
//		if(sunBeam){
//			sunBeam.SetActive(false);
//		}
	}
}
