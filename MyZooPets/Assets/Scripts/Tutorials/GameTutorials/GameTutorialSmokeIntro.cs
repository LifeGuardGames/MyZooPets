using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_SmokeIntro
// Tutorial that introduces the smoke monster.
//---------------------------------------------------

public class GameTutorialSmokeIntro : GameTutorial{
	private GameObject swipeGO; //reference to the swipe listener

	public GameTutorialSmokeIntro() : base(){		
	}

	protected override void SetMaxSteps(){
		maxSteps = 3;
	}
			
	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_SMOKE_INTRO;
	}
			
	protected override void _End(bool isFinished){
	}
			
	protected override void ProcessStep(int step){
		switch(step){
		case 0:
				// actually place the tutorial trigger dust behind the smoke monster now, even though this is for a later
				// tutorial...it's so the user can actually see it with the smoke monster
			DegradationUIManager.Instance.PlaceTutorialTrigger();
			
				// begin the panning "cut scene"
			TutorialManager.Instance.StartCoroutine(BeginPanRight());
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
	
	private void ShowWellapad(){
		// highlight the fight task
		WellapadMissionController.Instance.HighlightTask("FightMonster");
	
		// show the wellapad
		WellapadUIManager.Instance.OpenUI();
	
		// enable the close button		
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList(goBack);
		
		// listen for wellapad closing
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;			
		
	}

	/// <summary>
	/// Raises the wellapad closed event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnWellapadClosed(object sender, UIManagerEventArgs args){
		if(args.Opening == false){
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	

	/// <summary>
	/// Begins the intro for smoke monster.
	/// </summary>
	/// <returns>The intro.</returns>
	private IEnumerator BeginIntro(){
		// wait a brief moment
		float waitTime = Constants.GetConstant<float>("SmokeIntroWait");
		yield return new WaitForSeconds(waitTime);
		
		// play sound
		AudioManager.Instance.PlayClip("tutorialSmokeIntro");
	}

	/// <summary>
	/// This function handles the slight pan to view the smoke monster in the next room.
	/// </summary>
	/// <returns>The pan right.</returns>
	private IEnumerator BeginPanRight(){	
		// wait a brief moment
		float waitTime = Constants.GetConstant<float>("SmokeIntroWaitBetweenPans");
		yield return new WaitForSeconds(waitTime);
		
		// begin the pan right
		PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
		float moveTo = scriptPan.partitionOffset;
		float panTime = Constants.GetConstant<float>("SmokeIntroPanTime");
		
		/* // can't use lean tween callbacks because this is not a game object...curses
        Hashtable optional = new Hashtable();
		optional.Add ("onComplete", "OnRightPanDone");
		optional.Add("onCompleteTarget", gameObject);
		*/
		LeanTween.moveX(CameraManager.Instance.gameObject.transform.parent.gameObject, moveTo, panTime);
		
		yield return new WaitForSeconds(panTime);
		
		OnRightPanDone();
	}
	
	private void OnRightPanDone(){
		// 	begin pan to the left
		TutorialManager.Instance.StartCoroutine(BeginPanLeft());
	}
	
	private IEnumerator BeginPanLeft(){
		// wait a brief moment
		float waitTime = Constants.GetConstant<float>("SmokeIntroFocusTime");
		yield return new WaitForSeconds(waitTime);
		
		// begin the pan right
		float moveTo = 0f;
		float panTime = Constants.GetConstant<float>("SmokeIntroPanTime");
		
		/* // can't use lean tween callbacks because this is not a game object...curses
        Hashtable optional = new Hashtable();
		optional.Add ("onComplete", "OnLeftPanDone");
		optional.Add("onCompleteTarget", gameObject);
			*/
		LeanTween.moveX(CameraManager.Instance.gameObject.transform.parent.gameObject, moveTo, panTime); 
		
		OnLeftPanDone();
	}
	
	private void OnLeftPanDone(){
		Advance();	
	}
	
	/// <summary>
	/// Setups the swipe listener.
	/// Creates a giant collider on the screen that listens to the swipe event.
	/// This is an easier way to do swipe during tutorial
	/// </summary>
	private void SetupSwipeListener(){
		//check for right anchor
		GameObject anchorRight = GameObject.Find("Anchor-Right");
		string tutKey = GetKey() + "_" + GetStep();

		if(anchorRight == null)
			Debug.LogError(tutKey + " Needs anchor right");

		//spawn the giant collider
		GameObject swipeResource = (GameObject)Resources.Load("TutorialSwipeListener");
		swipeGO = NGUITools.AddChild(anchorRight, swipeResource);
		swipeGO.GetComponent<TutorialSwipeListener>().OnTutorialSwiped += OnTutorialSwiped;

		//show finger hint
		ShowFingerHint(anchorRight, true, fingerHintPrefab: "PressHoldSwipeTut");

		// show message
		Vector3 location = Constants.GetConstant<Vector3>("SmogIntroPopupLoc");
		string tutMessage = Localization.Localize(tutKey);
		Hashtable option = new Hashtable();

		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, location, useViewPort: false, option: option);
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