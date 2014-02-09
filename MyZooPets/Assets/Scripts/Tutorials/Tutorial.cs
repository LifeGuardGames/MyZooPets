using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Tutorial
// Parent class for all tutorials.
//---------------------------------------------------
public abstract class Tutorial {
	//---------------------Events--------------------------
	public EventHandler<TutorialEndEventArgs> OnTutorialEnd; // when the tutorial ends

	// ----------- Abstract functions -------------------
	protected abstract void SetKey();						// the tutorial key is used to mark a lot of lists
	protected abstract void SetMaxSteps();					// set the max steps of the tutorial
	protected abstract void ProcessStep( int nStep );		// the meat of a tutorial is processing its steps and doing things
	protected abstract void _End( bool bFinished );			// when the tutorial is finishd
	
	// ----------- Tutorial Popup types -------------------
	protected const string POPUP_STD = "TutorialPopup_Standard";
	protected const string POPUP_STD_WITH_IMAGE = "TutorialPopup_StandardWithImage";

	protected const string POPUP_LONG = "TutorialPopup_Long";
	protected const string POPUP_LONG_WITH_BUTTON = "TutorialPopup_LongWithButton";
	protected const string POPUP_LONG_WITH_BUTTON_AND_IMAGE = "TutorialPopup_LongWithButtonAndImage";
	
	protected int nMaxSteps; // max steps in the tutorial
	protected string strKey; // key for this tutorial
	protected Vector3 POS_TOP = new Vector3( 0, 242, 0 ); //top position to spawn the popup (NGUI)
	protected Vector3 POS_BOT = new Vector3( 0, -242, 0 ); //bottom position to spawn the popup (NGUI)

	private List<GameObject> listCanProcess = new List<GameObject>(); // list of objects that can be processed as input
	private GameObject goSpotlight;	// current (and only) spotlight object this tutorial is highlighting
	private GameObject goPopup; // current (and only) tutorial popup
	private GameObject goFingerHint; //current finger hint
	private int nCurrentStep; // step the tutorial is currently on

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

	//Return the current step that the tutorial is on
	public int GetStep() {
		return nCurrentStep;	
	}

	//Set the tutorial to a specific step
	protected void SetStep( int num ) {
		nCurrentStep = num;
		
		// if we have exceeded max steps in this tutorial, end it
		if ( nCurrentStep >= nMaxSteps )
			End( true );
		else
			ProcessStep( nCurrentStep );
	}

	//Return the key of this tutorial
	protected string GetKey() {
		if ( string.IsNullOrEmpty(strKey) )
			SetKey();
		
		return strKey;	
	}
	
	public Tutorial() {
		// Debug.Log("Starting tutorial " + GetKey());
		SetMaxSteps();
		SetStep( 0 );
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
	// Abort()
	// Ends the tutorial early.
	//---------------------------------------------------		
	public void Abort() {
		End( false );	
	}
	
	//---------------------------------------------------
	// End()
	// When this tutorial is finished.
	//---------------------------------------------------		
	protected virtual void End( bool bFinished ) {
		// debug message
		// Debug.Log("Tutorial Ending: " + GetKey());
		
		// let children know the tutorial is over
		_End( bFinished );
		
		// save the fact that the user completed this tutorial
		if ( bFinished ){
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add( GetKey() );
			Analytics.Instance.TutorialCompleted(GetKey());
		}

		if (goPopup != null)
			GameObject.Destroy(goPopup);
		
		// activate tutorial end callback
		if( OnTutorialEnd != null )
        	OnTutorialEnd(this, new TutorialEndEventArgs( bFinished ) );	
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
	protected void SpotlightObject( GameObject goTarget, bool bGUI = false, 
		InterfaceAnchors eAnchor = InterfaceAnchors.Center, string strSpotlightPrefab = "TutorialSpotlight",
		bool fingerHint = false, float fingerHintOffsetFromSpotlighCenter = 60.0f){
		// get the proper location of the object we are going to focus on
		Vector3 vPos;
		if ( bGUI )
			vPos = LgNGUITools.GetScreenPosition( goTarget );
		else {
			// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into NGUI center
			vPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.cameraMain, goTarget.transform.position);
			// Camera.main.WorldToScreenPoint( goTarget.transform.position );

			vPos = CameraManager.Instance.TransformAnchorPosition( vPos, 
				InterfaceAnchors.BottomLeft, InterfaceAnchors.Center );
		}
		
		// destroy the old object if it existed
		if (goSpotlight != null)
			GameObject.Destroy(goSpotlight);

		if(goFingerHint != null)
			GameObject.Destroy(goFingerHint);
		
		// create the spotlight
		GameObject goResource = Resources.Load( strSpotlightPrefab ) as GameObject;
		string strAnchor = "Anchor-" + eAnchor.ToString();
		goSpotlight = LgNGUITools.AddChildWithPosition( GameObject.Find(strAnchor), goResource );
		
		// move the spotlight into position
		vPos.z = goSpotlight.transform.localPosition.z; // keep the default z-value of the spotlight
		goSpotlight.transform.localPosition = vPos;

		// spawn finger hint
		if(fingerHint){
			GameObject fingerHintResource = (GameObject) Resources.Load("DegradationPressTut");
			goFingerHint = LgNGUITools.AddChildWithPosition(GameObject.Find(strAnchor), fingerHintResource);
			vPos.z = goFingerHint.transform.localPosition.z;
			vPos.y = vPos.y + fingerHintOffsetFromSpotlighCenter; //offset in Y so the finger hint doesn't overlap the image
			goFingerHint.transform.localPosition = vPos;
		}
	}

	protected void RemoveFingerHint(){
		if(goFingerHint == null){
			Debug.LogError("Trying to destroy a finger hint that doesn't exist (" + GetKey() + " -- " + GetStep() +")");
			return;
		}

		GameObject.Destroy(goFingerHint);
	}
	
	//---------------------------------------------------
	// RemoveSpotlight()
	// Removes the current spotlight object.
	//---------------------------------------------------		
	protected void RemoveSpotlight() {
		if ( goSpotlight == null ) {
			Debug.LogError("Trying to destroy a spotlight that doesn't exist (" + GetKey() + " -- " + GetStep() +")");
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
	// Display the tutorial popup
	// Option Params:
	//	Message(string): the text you want to display
	//	SpriteAtlas(string): name of the atlas that the sprite is from. Required if loading an image
	//	SpriteName(string): name of the image
	//	Button1Callback(function): action to do when button is clicked
	//	Button1Label(string): what does the button say
	//	ShrinkBgToFitText(bool): default to T. background size is automatically adjusted to fit label
	//---------------------------------------------------
	protected void ShowPopup(string popupKey, Vector3 vLoc, bool useViewPort=true, Hashtable option=null){
		if(goPopup)
			GameObject.Destroy(goPopup);

		if(option == null)
			option = new Hashtable();

		Vector3 vPos = vLoc;

		if(!option.ContainsKey(TutorialPopupFields.Message)){
			// get text to display from tutorial key + step
			string strText = Localization.Localize(GetKey() + "_" + GetStep());
			option.Add(TutorialPopupFields.Message, strText);
		}

		if(!option.ContainsKey(TutorialPopupFields.ShrinkBgToFitText))
			option.Add(TutorialPopupFields.ShrinkBgToFitText, false);

		if(useViewPort)	{
			// transform viewport location to screen position, then from bottom left to center
			vPos = CameraManager.Instance.ViewportToScreen(CameraManager.Instance.cameraMain, vLoc);
			vPos = CameraManager.Instance.TransformAnchorPosition( vPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center );
			//Debug.Log("Viewport: " + vLoc + " to Screen: " + vPos );
		}

		// create the popup
		GameObject goResource = Resources.Load(popupKey) as GameObject;
		goPopup = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-Center"), goResource);
	 	vPos.z = goPopup.transform.position.z; // keep the default z-value
		goPopup.transform.localPosition = vPos;				
		
		//feed the script the option hashtable		
		TutorialPopup script = goPopup.GetComponent<TutorialPopup>();
		script.Init(option);
	}
}
