using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_SmokeIntro
// Tutorial that introduces the smoke monster.
//---------------------------------------------------

public class GameTutorial_SmokeIntro : GameTutorial {
	
	public GameTutorial_SmokeIntro() : base() {		
	}
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 2;
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
				TutorialManager.Instance.StartCoroutine( BeginIntro() );
				break;
			
			case 1:
				TutorialManager.Instance.StartCoroutine( BeginPanRight() );
				break;
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
		
		// show the comic & listen for callback when it is done
		GameObject resourceMovie = Resources.Load("Cutscene_SmokeIntro" ) as GameObject;
		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
		CutsceneFrames.OnCutsceneDone += CutsceneDone;
	}
	
	//---------------------------------------------------
	// CutsceneDone()
	//---------------------------------------------------		
    private void CutsceneDone(object sender, EventArgs args){	
		// unsub from callback
		CutsceneFrames.OnCutsceneDone -= CutsceneDone;
				
		// advance the tutorial
		Advance();
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
}
