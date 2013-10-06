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
			End( true );
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
		if ( string.IsNullOrEmpty(strKey) )
			SetKey();
		
		return strKey;	
	}
	protected virtual void SetKey() {
		// children should implement this	
	}
	
	// the UI element used to show the tutorial message
	private TutorialMessage scriptMessage;
	protected Vector3 POS_TOP = new Vector3( 0, 242, 0 );
	protected Vector3 POS_BOT = new Vector3( 0, -242, 0 );

	//=======================Events========================
	public EventHandler<TutorialEndEventArgs> OnTutorialEnd; // when the tutorial ends
	//=====================================================		
	
	public MinigameTutorial() {
		SetStep( 0 );
	}
	
	//---------------------------------------------------
	// ShowMessage()
	// Shows message for this part of the tutorial.
	//---------------------------------------------------	
	protected void ShowMessage( string strResourceKey, Vector3 vPos ) {
		// get the text to display based on the current step and the tutorial's key
		int nStep = GetStep();
		string strTutKey = GetKey();
		string strKey = strTutKey + "_" + nStep;
		string strText = Localization.Localize( strKey );
		
		// if there was an existing message but its key does not match the incoming key, destroy its game object
		if ( scriptMessage != null && scriptMessage.GetResourceKey() != strResourceKey ) {
			GameObject.Destroy( scriptMessage.gameObject );
			scriptMessage = null;
		}
		
		// if the panel doesn't exist, create it
		if ( scriptMessage == null ) {
			GameObject anchorCenter = GameObject.Find("Anchor-Center");
			GameObject prefab = Resources.Load(strResourceKey) as GameObject;
			GameObject goMessage = LgNGUITools.AddChildWithPosition( anchorCenter, prefab );
			scriptMessage = goMessage.GetComponent<TutorialMessage>();
			
			// set variables
			scriptMessage.SetTutorial( this );
			scriptMessage.SetResourceKey( strResourceKey );			
		}
		
		// set the position
		scriptMessage.SetPosition( vPos );
		
		// set text
		scriptMessage.SetLabel( strText );
	}
	
	//---------------------------------------------------
	// Advance()
	// Go to the next part of this tutorial.
	//---------------------------------------------------	
	public void Advance() {
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
	private void End( bool bFinished ) {
		// let children know the tutorial is over
		_End( bFinished );
		
		// save the fact that the user completed this tutorial
		if ( bFinished )
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add( GetKey() );
		
		// if there are any messages showing, destroy them
		if ( scriptMessage != null )
			GameObject.Destroy( scriptMessage.gameObject );
		
		// activate tutorial end callback
		if( OnTutorialEnd != null )
        	OnTutorialEnd(this, new TutorialEndEventArgs( bFinished ) );	
	}
	
	//---------------------------------------------------
	// Abort()
	// Ends the tutorial early.
	//---------------------------------------------------		
	public void Abort() {
		End( false );	
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected virtual void _End( bool bFinished ) {
		Debug.Log("Base tutorial _End() should not be getting called");	
	}
}
