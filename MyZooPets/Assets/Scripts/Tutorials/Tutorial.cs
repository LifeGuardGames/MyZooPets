using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Tutorial
// Parent class for all tutorials.
//---------------------------------------------------

public abstract class Tutorial {
	// ----------- Abstract functions -------------------
	protected abstract void SetKey();						// the tutorial key is used to mark a lot of lists
	protected abstract void SetMaxSteps();					// set the max steps of the tutorial
	protected abstract void ProcessStep( int nStep );		// the meat of a tutorial is processing its steps and doing things
	protected abstract void _End( bool bFinished );			// when the tutorial is finishd
	// --------------------------------------------------
	
	// list of objects that can be processed as input
	private List<GameObject> listCanProcess = new List<GameObject>();
	protected void AddToProcessList( GameObject go ) {
		listCanProcess.Add( go );
	}
	protected void RemoveFromProcessList( GameObject go ) {
		listCanProcess.Remove( go );	
	}
	public bool CanProcess( GameObject go ) {
		bool bCan = listCanProcess.Contains( go );
		return bCan;
	}
	
	// current (and only) spotlight object this tutorial is highlighting
	private GameObject goSpotlight;	
	
	// step the tutorial is currently on
	private int nCurrentStep;
	protected void SetStep( int num ) {
		nCurrentStep = num;
		
		// if we have exceeded max steps in this tutorial, end it
		if ( nCurrentStep >= nMaxSteps )
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
	
	// the UI element used to show the tutorial message
	private TutorialMessage scriptMessage;
	protected Vector3 POS_TOP = new Vector3( 0, 242, 0 );
	protected Vector3 POS_BOT = new Vector3( 0, -242, 0 );

	//=======================Events========================
	public EventHandler<TutorialEndEventArgs> OnTutorialEnd; // when the tutorial ends
	//=====================================================		
	
	public Tutorial() {
		SetMaxSteps();
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
	// End()
	// When this tutorial is finished.
	//---------------------------------------------------		
	protected virtual void End( bool bFinished ) {
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
	// SpotlightObject()
	// Puts a spotlight around the incoming object to
	// draw attention to it.
	//---------------------------------------------------	
	protected void SpotlightObject( GameObject goTarget ) {
		// if the spotlight object already exists, then we want to just move it to the new location
		if ( goSpotlight != null ) {
			MoveSpotlight( goTarget );
			return;
		}

		// get the proper location of the object we are going to focus on
		Vector3 vPos = Camera.main.WorldToScreenPoint( goTarget.transform.position );
		
		// create the object
		GameObject goResource = Resources.Load( "TutorialSpotlight" ) as GameObject;
		goSpotlight = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-BottomLeft"), goResource );
		vPos.z = goSpotlight.transform.position.z; // keep the default z-value of the spotlight
		goSpotlight.transform.localPosition = vPos;
	}
	
	//---------------------------------------------------
	// MoveSpotlight()
	// Moves the spotlight object to focus on goTarget.
	//---------------------------------------------------		
	private void MoveSpotlight( GameObject goTarget ) {
		Debug.Log("Moving spotlight code not yet implemented.  Sorry bro.");
	}
	
	//---------------------------------------------------
	// RemoveSpotlight()
	// Removes the current spotlight object.
	//---------------------------------------------------		
	protected void RemoveSpotlight() {
		if ( goSpotlight == null ) {
			Debug.Log("Trying to destroy a spotlight that doesn't exist!");
			return;
		}
		
		GameObject.Destroy( goSpotlight );
	}	
}
