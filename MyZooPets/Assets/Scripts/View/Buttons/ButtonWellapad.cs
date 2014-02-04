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

		//Listens to update event from wellapad mission controller
		WellapadMissionController.Instance.OnTaskUpdated += EnableButtonBounce;
		WellapadMissionController.Instance.OnMissionsRefreshed += EnableButtonBounce;
	}
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		if ( WellapadUIManager.Instance.IsOpen() )
			WellapadUIManager.Instance.CloseUI();
		else{
			WellapadUIManager.Instance.OpenUI();
			DisableButtonBounce();
		}
	}

	private void EnableButtonBounce(object sender, EventArgs args){
		AnimationControl animControl = GetComponent<AnimationControl>();
		if(animControl != null)
			animControl.Play("smallBounceSoftWellapad");
	}

	private void DisableButtonBounce(){
		AnimationControl animControl = GetComponent<AnimationControl>();
		if(animControl.IsPlaying("smallBounceSoftWellapad"))
			animControl.StopAndResetFrame("zero");
	}
}
