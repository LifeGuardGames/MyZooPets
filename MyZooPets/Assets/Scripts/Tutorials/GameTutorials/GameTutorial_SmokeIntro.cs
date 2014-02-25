using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_SmokeIntro
// Tutorial that introduces the smoke monster.
//---------------------------------------------------

public class GameTutorial_SmokeIntro : GameTutorial {
	private GameObject swipeGO; //reference to the swipe listener

	public GameTutorial_SmokeIntro() : base() {		
	}
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 3;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		strKey = TutorialManager_Bedroom.TUT_SMOKE_INTRO;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected override void _End( bool bFinished ) {
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		switch ( nStep ) {
			case 0:
				// actually place the tutorial trigger dust behind the smoke monster now, even though this is for a later
				// tutorial...it's so the user can actually see it with the smoke monster
				DegradationUIManager.Instance.PlaceTutorialTrigger();
			
				// begin the panning "cut scene"
				TutorialManager.Instance.StartCoroutine( BeginPanRight() );
				break;
			
			case 1:
				// open the wellapad to show the user what to do next
				ShowWellapad();
				break;

			case 2:
				SetupSwipeListener();
				break;
		}
	}

	
	
	//---------------------------------------------------
	// ShowWellapad()
	//---------------------------------------------------		
	private void ShowWellapad() {
		// highlight the fight task
		WellapadMissionController.Instance.HighlightTask( "FightMonster" );
	
		// show the wellapad
		WellapadUIManager.Instance.OpenUI();
	
		// enable the close button		
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList( goBack );
		
		// listen for wellapad closing
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;			
		
	}
	
	//---------------------------------------------------
	// OnWellapadClosed()
	// Callback for when the wellapad is closed.
	//---------------------------------------------------	
	private void OnWellapadClosed( object sender, UIManagerEventArgs args ) {
		if ( args.Opening == false ) {
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	
	
	//---------------------------------------------------
	// BeginIntro()
	// Starts the intro for the some monster tutorial.
	//---------------------------------------------------		
	private IEnumerator BeginIntro() {
		// wait a brief moment
		float fWait = Constants.GetConstant<float>( "SmokeIntroWait" );
		yield return new WaitForSeconds( fWait );
		
		// play sound
		AudioManager.Instance.PlayClip( "tutorialSmokeIntro" );
	}
	
	//---------------------------------------------------
	// BeginPanRight()
	// This function handles the slight pan to view the
	// smoke monster in the next room over.
	//---------------------------------------------------		
	private IEnumerator BeginPanRight() {	
		// wait a brief moment
		float fWait = Constants.GetConstant<float>( "SmokeIntroWaitBetweenPans" );
		yield return new WaitForSeconds( fWait );
		
		// begin the pan right
		PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
		float fMoveTo = scriptPan.partitionOffset;
		float fTime = Constants.GetConstant<float>( "SmokeIntroPanTime" );
		
		/* // can't use lean tween callbacks because this is not a game object...curses
        Hashtable optional = new Hashtable();
		optional.Add ("onComplete", "OnRightPanDone");
		optional.Add("onCompleteTarget", gameObject);
		*/
        LeanTween.moveX(CameraManager.Instance.gameObject.transform.parent.gameObject, fMoveTo, fTime );
		
		yield return new WaitForSeconds( fTime );
		
		OnRightPanDone();
	}
	
	private void OnRightPanDone() {
		// 	begin pan to the left
		TutorialManager.Instance.StartCoroutine( BeginPanLeft() );
	}
	
	private IEnumerator BeginPanLeft() {
		// wait a brief moment
		float fWait = Constants.GetConstant<float>( "SmokeIntroFocusTime" );
		yield return new WaitForSeconds( fWait );
		
		// begin the pan right
		float fMoveTo = 0f;
		float fTime = Constants.GetConstant<float>( "SmokeIntroPanTime" );
		
		/* // can't use lean tween callbacks because this is not a game object...curses
        Hashtable optional = new Hashtable();
		optional.Add ("onComplete", "OnLeftPanDone");
		optional.Add("onCompleteTarget", gameObject);
			*/
        LeanTween.moveX(CameraManager.Instance.gameObject.transform.parent.gameObject, fMoveTo, fTime ); 
		
		OnLeftPanDone();
	}
	
	private void OnLeftPanDone() {
		Advance();	
	}

	//---------------------------------------------------	
	// SetupSwipeListener()
	// Creates a giant collider on the screen that listens to
	// the swipe event. 
	// This is an easier way to do swipe during tutorial.
	//---------------------------------------------------	
	private void SetupSwipeListener(){
		//check for right anchor
		GameObject anchorRight = GameObject.Find("Anchor-Right");
		if(anchorRight == null)
			Debug.LogError(GetKey() + " -- " + GetStep() + " Needs anchor right");

		//spawn the giant collider
		GameObject swipeResource = (GameObject) Resources.Load("TutorialSwipeListener");
		swipeGO = NGUITools.AddChild(anchorRight, swipeResource);
		swipeGO.GetComponent<TutorialSwipeListener>().OnTutorialSwiped += OnTutorialSwiped;

		//show finger hint
		ShowFingerHint(anchorRight, true, fingerHintPrefab:"PressHoldSwipeTut");
	}

	private void OnTutorialSwiped(object sender, EventArgs args){
		//clean up
		RemoveFingerHint();

		if(swipeGO != null)
			GameObject.Destroy(swipeGO);

		//advance in tutorial
		Advance();
	}
}
