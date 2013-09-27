using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// MinigameTutorial
// Parent class for all minigame tutorials.
//---------------------------------------------------

public class MinigameTutorial {
	
	// step the tutorial is currently on
	private int nCurrentStep;
	protected void SetStep( int num ) {
		nCurrentStep = num;
		
		// if we have exceeded max steps in this tutorial, etnd it
		if ( nCurrentStep > nMaxSteps )
			End();
		else
			ProcessStep( nCurrentStep );
	}
	protected int GetStep() {
		return nCurrentStep;	
	}
	
	// max steps in the tutorial
	protected int nMaxSteps;
	
	// key for this tutorial
	protected string strKey;
	protected string GetKey() {
		return strKey;	
	}
	
	//=======================Events========================
	public EventHandler<EventArgs> OnTutorialEnd; // when the tutorial ends
	//=====================================================		
	
	public MinigameTutorial() {
		SetStep( 0 );
	}
	
	//---------------------------------------------------
	// ShowMessage()
	// Shows message for this part of the tutorial.
	//---------------------------------------------------	
	protected void ShowMessage() {
		// get the text to display based on the current step and the tutorial's key
		int nStep = GetStep();
		string strTutKey = GetKey();
		string strKey = strTutKey + "_" + nStep;
		string strText = Localization.Localize( strKey );
		
		// if the panel to display text isn't showing, show it
		
		// set the text on the panel
	}
	
	//---------------------------------------------------
	// Advance()
	// Go to the next part of this tutorial.
	//---------------------------------------------------	
	protected void Advance() {
		// increment the current step of the tutorial
		int nStep = GetStep();
		nStep++;
		SetStep( nStep );
	}
	
	//---------------------------------------------------
	// ProcessStep()
	// Children classes implement this as a way to do
	// the actual behavior on a tutorial step.
	//---------------------------------------------------		
	protected virtual void ProcessStep( int nStep ) {
		Debug.Log("Base tutorial should not be processing a step.");	
	}
	
	//---------------------------------------------------
	// End()
	// When this tutorial is finished.
	//---------------------------------------------------		
	private void End() {
		// let children know the tutorial is over
		_End();
		
		// activate tutorial end callback
		if( OnTutorialEnd != null )
        	OnTutorialEnd(this, EventArgs.Empty);	
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected virtual void _End() {
		Debug.Log("Base tutorial _End() should not be getting called");	
	}
}
