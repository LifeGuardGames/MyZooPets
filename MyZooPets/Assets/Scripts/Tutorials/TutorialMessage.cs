using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// TutorialMessage
// Gameobject on NGUI panels designed for showing
// messages to the player during tutorials.
// Visually different from TutorialPopup
//---------------------------------------------------	
public class TutorialMessage : MonoBehaviour {
	public UILabel label; // the label used to hold the message

	private string strResourceKey; // the resource key that this object was created with
	private Tutorial scriptTutorial; // minigame tutorial that this message belongs to

	public void SetResourceKey( string strKey ) {
		strResourceKey = strKey;	
	}

	public string GetResourceKey() {
		return strResourceKey;	
	}
	
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
	}
	
	//---------------------------------------------------
	// SetPosition()
	// Sets the position of this panel.
	//---------------------------------------------------		
	public void SetPosition( Vector3 vPos ) {
		gameObject.transform.localPosition = vPos;
	}
}
