using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// ButtonWellapad
// Button that opens the Wellapad.
//---------------------------------------------------

public class ButtonWellapad : LgButton {
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------		
	protected override void _Start() {		
		// for debug/testing -- we may have the wellapad disabled
		bool bOK = Constants.GetConstant<bool>( "WellapadOn" );
		if ( !bOK )
			NGUITools.SetActive( gameObject, false );

		bool tutDone = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
		if(tutDone){
			//Listens to update event from wellapad mission controller
			WellapadMissionController.Instance.OnTaskUpdated += EnableButtonBounce;
			WellapadMissionController.Instance.OnMissionsRefreshed += EnableButtonBounce;

			//Start bouncing if there are active tasks
			if(WellapadMissionController.Instance.HasActiveTasks())
				EnableButtonBounce(this, EventArgs.Empty);
		}
	}
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		if(WellapadUIManager.Instance.IsOpen())
			WellapadUIManager.Instance.CloseUI();
		else{
			WellapadUIManager.Instance.OpenUI();
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
		AnimationControl animControl = GetComponent<AnimationControl>();
		if(animControl != null)
			animControl.Play("smallBounceSoftNav");
	}

	private void DisableButtonBounce(){
		AnimationControl animControl = GetComponent<AnimationControl>();
		if(animControl.IsPlaying("smallBounceSoftNav"))
			animControl.StopAndResetFrame("zero");
	}
}
