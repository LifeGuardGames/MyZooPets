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
	
	// ----------- Tutorial Popup types -------------------
	protected const string POPUP_STD = "TutorialPopup_Standard";
	protected const string POPUP_LONG = "TutorialPopup_Long";

	//add popup prefab here
	protected const string POPUP_RUNNER = "TutorialPopup_Runner";

	// ----------------------------------------------------
	
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
	
	// current (and only) tutorial popup
	private GameObject goPopup;
	
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
		Debug.Log("Starting tutorial " + GetKey());
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
		// debug message
		Debug.Log("Tutorial Ending: " + GetKey());
		
		// let children know the tutorial is over
		_End( bFinished );
		
		// save the fact that the user completed this tutorial
		if ( bFinished ){
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add( GetKey() );
			Analytics.Instance.TutorialCompleted(GetKey());
		}
		
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
	// eAnchor is the incoming anchor of the object/where
	// the spotlight should be created.  For 3D objects
	// the anchor should be center, and for GUI elements
	// the anchor should be whatever anchor the element
	// is in.
	//---------------------------------------------------	
	protected void SpotlightObject( GameObject goTarget, bool bGUI = false, InterfaceAnchors eAnchor = InterfaceAnchors.Center, string strSpotlightPrefab = "TutorialSpotlight" ) {
		// get the proper location of the object we are going to focus on
		Vector3 vPos;
		if ( bGUI )
			vPos = LgNGUITools.GetScreenPosition( goTarget );
		else {
			// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into NGUI center
			vPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.cameraMain, goTarget.transform.position);
			// Camera.main.WorldToScreenPoint( goTarget.transform.position );

			vPos = CameraManager.Instance.TransformAnchorPosition( vPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center );
		}
		
		// destroy the old object if it existed
		if ( goSpotlight != null )
			GameObject.Destroy( goSpotlight );
		
		// create the spotlight
		GameObject goResource = Resources.Load( strSpotlightPrefab ) as GameObject;
		string strAnchor = "Anchor-" + eAnchor.ToString();
		goSpotlight = LgNGUITools.AddChildWithPosition( GameObject.Find(strAnchor), goResource );
		
		// move the spotlight into position
		vPos.z = goSpotlight.transform.localPosition.z; // keep the default z-value of the spotlight
		goSpotlight.transform.localPosition = vPos;
	}
	
	//---------------------------------------------------
	// RemoveSpotlight()
	// Removes the current spotlight object.
	//---------------------------------------------------		
	protected void RemoveSpotlight() {
		if ( goSpotlight == null ) {
			Debug.LogError("Trying to destroy a spotlight that doesn't exist!");
			return;
		}
		
		GameObject.Destroy( goSpotlight );
	}	
	
	//---------------------------------------------------
	// RemovePopup()
	// Removes the current popup object.
	//---------------------------------------------------		
	protected void RemovePopup() {
		if ( goPopup == null ) {
			Debug.LogError("Trying to destroy a popup that doesn't exist!");
			return;
		}
		
		GameObject.Destroy( goPopup );
	}	
	
	//---------------------------------------------------
	// ShowPopup()
	//---------------------------------------------------	
	protected void ShowPopup( string strPopupKey, Vector3 vLoc, bool useViewPort=true ) {
		// if there was already a popup, just destroy it
		if ( goPopup )
			GameObject.Destroy( goPopup );
		
		// get text to display from tutorial key + step
		string strText = Localization.Localize( GetKey() + "_" + GetStep() );
		Vector3 vPos = vLoc;
	
		if(useViewPort)	{
			// transform viewport location to screen position, then from bottom left to center
			vPos = CameraManager.Instance.ViewportToScreen(CameraManager.Instance.cameraMain, vLoc);
			vPos = CameraManager.Instance.TransformAnchorPosition( vPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center );
			//Debug.Log("Viewport: " + vLoc + " to Screen: " + vPos );
		}
		
		// create the popup
		GameObject goResource = Resources.Load( strPopupKey ) as GameObject;
		goPopup = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), goResource );
		vPos.z = goPopup.transform.position.z; // keep the default z-value
		goPopup.transform.localPosition = vPos;	
		
		TutorialPopup script = goPopup.GetComponent<TutorialPopup>();
		script.Init( strText );
	}

	protected void ShowPopup(GameObject tutorialPopup, Vector3 vLoc, bool useViewPort=true){
		// if there was already a popup, just destroy it
		if ( goPopup )
			GameObject.Destroy( goPopup );
		
		Vector3 vPos = vLoc;
	
		if(useViewPort)	{
			// transform viewport location to screen position, then from bottom left to center
			vPos = CameraManager.Instance.ViewportToScreen(CameraManager.Instance.cameraMain, vLoc);
			vPos = CameraManager.Instance.TransformAnchorPosition( vPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center );
		}

		tutorialPopup.transform.parent = GameObject.Find("Anchor-Center").transform;
		vPos.z = -42; //quick hack to make sure the GO is closer to camera
		tutorialPopup.transform.localPosition = vPos;
		tutorialPopup.transform.localScale = new Vector3(1, 1, 1);
		goPopup = tutorialPopup;
	}
}
