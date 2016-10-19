using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//---------------------------------------------------
// GameTutorial_SmokeIntro
// Tutorial that introduces the smoke monster.
//---------------------------------------------------
public class GameTutorialSmokeIntro : GameTutorial{
	private Button rightArrowButton;

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
			TutorialManager.Instance.StartCoroutine(BeginPanRight());
			break;
		case 1:
			// open the wellapad to show the user what to do next
			ShowWellapad();
			break;
		case 2:
			FocusOnRightArrow();
			break;
		}
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
	/// This function handles the slight pan to view the smoke monster in the next room.
	/// </summary>
	/// <returns>The pan right.</returns>
	private IEnumerator BeginPanRight(){
		// wait a brief moment
		float waitTime = Constants.GetConstant<float>("SmokeIntroWaitBetweenPans");
		yield return new WaitForSeconds(waitTime);
		
		// begin the pan right
		PanToMoveCamera scriptPan = CameraManager.Instance.PanScript;
		float moveTo = scriptPan.partitionOffset;
		float panTime = Constants.GetConstant<float>("SmokeIntroPanTime");

		LeanTween.moveX(CameraManager.Instance.gameObject.transform.parent.gameObject, moveTo, panTime)
			.setEase(LeanTweenType.easeInOutQuad);
		
		yield return new WaitForSeconds(panTime);
		
		OnRightPanDone();
	}
	
	private void OnRightPanDone(){
		// begin pan to the left
		TutorialManager.Instance.StartCoroutine(BeginPanLeft());
	}
	
	private IEnumerator BeginPanLeft(){
		// wait a brief moment
		float waitTime = Constants.GetConstant<float>("SmokeIntroFocusTime");
		yield return new WaitForSeconds(waitTime);
		
		// begin the pan right
		float moveTo = 0f;
		float panTime = Constants.GetConstant<float>("SmokeIntroPanTime");

		LeanTween.moveX(CameraManager.Instance.gameObject.transform.parent.gameObject, moveTo, panTime)
			.setEase(LeanTweenType.easeInOutQuad); 

		OnLeftPanDone();
	}
	
	private void OnLeftPanDone(){
		Advance();	
	}

	private void ShowWellapad(){
		// show the wellapad
		FireCrystalUIManager.Instance.OpenUIBasedOnScene();
		
		// enable the close button		
		GameObject goBack = WellapadUIManager.Instance.GetScreenManager().GetBackButton();
		AddToProcessList(goBack);
		
		// listen for wellapad closing
		WellapadUIManager.Instance.OnManagerOpen += OnWellapadClosed;	
	}

	private void FocusOnRightArrow(){
		//Start showing the right arrow
		RoomArrowsUIManager.Instance.ShowRightArrowOnly();

		// begin listening for when the inhaler is clicked
		rightArrowButton = RoomArrowsUIManager.Instance.RightArrowObject;

		// the inhaler is the only object that can be clicked
		AddToProcessList(rightArrowButton.gameObject, true);

		rightArrowButton.onClick.AddListener(() => { RightArrowClicked(); });

		// spotlight the arrow
		SpotlightObject(isGUI: true, GUIanchor: InterfaceAnchors.Right,
						hasFingerHint: true, fingerState: BedroomTutFingerController.FingerState.DelayPress,
						spotlightOffsetX: -40, spotlightOffsetY: 70, fingerHintFlip: false, delay: 0f);

		Vector3 location = Constants.GetConstant<Vector3>("SmogIntroPopupLoc");

		ShowPopup(TUTPOPUPTEXT, location);

		ShowRetentionPet(true, new Vector3(-281, -86, -160));
	}

	private void RightArrowClicked(){
		rightArrowButton.onClick.RemoveListener(() => { RightArrowClicked(); });

		RemoveSpotlight();
		RemoveFingerHint();
		RemovePopup();
		RemoveRetentionPet();
		Advance();
	}
}
