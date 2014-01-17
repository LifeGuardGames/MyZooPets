using UnityEngine;
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
	}
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		if ( WellapadUIManager.Instance.IsOpen() )
			WellapadUIManager.Instance.CloseUI();
		else
			WellapadUIManager.Instance.OpenUI();
	}
}
