using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// TutorialMessage
// Gameobject on NGUI panels designed for showing
// messages to the player during tutorials.
//---------------------------------------------------	

public class TutorialMessage : MonoBehaviour {

	// the label used to hold the message
	public UILabel label;
	
	// the resource key that this object was created with
	private string strResourceKey;
	public void SetResourceKey( string strKey ) {
		strResourceKey = strKey;	
	}
	public string GetResourceKey() {
		return strResourceKey;	
	}
	
	// minigame tutorial that this message belongs to
	private Tutorial scriptTutorial;
	public void SetTutorial( Tutorial tut ) {
		scriptTutorial = tut;	
	}
	
	//---------------------------------------------------
	// SetLabel()
	// Sets the text on the message's label.  Assumes the
	// text has already gone through localization.
	//---------------------------------------------------		
	public void SetLabel( string strText ) {
		label.text = strText;	
	}
	
	//---------------------------------------------------
	// Advance()
	// Advances the tutorial that this message belongs to.
	//---------------------------------------------------		
	private void Advance() {
		scriptTutorial.Advance();	
		Destroy( gameObject );
	}
	
	//---------------------------------------------------
	// SetPosition()
	// Sets the position of this panel.
	//---------------------------------------------------		
	public void SetPosition( Vector3 vPos ) {
		gameObject.transform.localPosition = vPos;
	}
}
