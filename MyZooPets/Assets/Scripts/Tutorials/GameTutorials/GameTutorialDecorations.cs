using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_Decorations
// Tutorial to explain decoration system.
//---------------------------------------------------

public class GameTutorialDecorations : GameTutorial{
	// decoration node for tutorial
	private GameObject goNode;
	private GameObject goExitButton; //reference to deco mode exit button
	
	public GameTutorialDecorations() : base(){	
	}	
			
	protected override void SetMaxSteps(){
		maxSteps = 5;
	}
			
	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_DECOS;
	}
			
	protected override void _End(bool isFinished){
		// since this is the last tutorial, show a little notification
		string strKey = "TUTS_FINISHED";											// key of text to show
		string strImage = Constants.GetConstant<string>("Tutorial_Finished");		// image to appear on notification
		string strAnalytics = "";														// analytics tracker

		// show the standard popup
		string petName = DataManager.Instance.GameData.PetInfo.PetName;
		TutorialUIManager.AddStandardTutTip(NotificationPopupType.TipWithImage, 
			String.Format(Localization.Localize(strKey), 
			StringUtils.FormatStringPossession(petName)),
			strImage, null, true, true, strAnalytics);

		GameObject wellapadButton = (GameObject)GameObject.Find("WellapadButton");
		if(wellapadButton != null){
			ButtonWellapad buttonWellapadScript = wellapadButton.GetComponent<ButtonWellapad>();
			buttonWellapadScript.SetListenersToWellapadMissionController();
		}
		else{
			Debug.LogError("wellapad button can't be found: " + this);
		}

	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			ShowWellapad();
			break;
		case 1:
			TutorialManager.Instance.StartCoroutine(FocusOnEditButton());
			break;
		case 2:
			FocusOnNode();
			break;
		case 3:
			TutorialManager.Instance.StartCoroutine(FocusOnDecorationUI());
			break;			
		case 4:
			TutorialManager.Instance.StartCoroutine(FocusOnDecoExitButton());
			break;
		}
	}
	
	private void ShowWellapad(){
		// highlight the fight task
		WellapadMissionController.Instance.HighlightTask("Decorate");
	
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
	/// Focuses the on edit button. Highlights the edit button
	/// </summary>
	/// <returns>The on edit button.</returns>
	private IEnumerator FocusOnEditButton(){
		// wait a brief moment
		float fWait = Constants.GetConstant<float>("DecoIntroWait");
		yield return new WaitForSeconds(fWait);

		//show message
		Vector3 location = Constants.GetConstant<Vector3>("DecorationPopupLoc");
		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);

		Hashtable option = new Hashtable();
		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, location, useViewPort: false, option: option);
		
		// find and spotlight the edit button
		GameObject goEditButton = NavigationUIManager.Instance.GetEditDecoButton();
		SpotlightObject(goEditButton, true, InterfaceAnchors.BottomLeft, fingerHint: true,
			fingerHintFlip: true, delay: 0.5f);
		
		// add the button to the process list so the user can click it
		AddToProcessList(goEditButton);
		
		// sign up for a callback for when the button is clicked
		EditDecosUIManager.Instance.OnManagerOpen += OnEditDecos;
	}	
	
	/// <summary>
	/// After Edit deco mode has started. clean up and advance to next tutorial step
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnEditDecos(object sender, UIManagerEventArgs args){
		// stop listening for callback
		EditDecosUIManager.Instance.OnManagerOpen -= OnEditDecos;

		// clean up
		RemoveSpotlight();
		RemovePopup();
		RemoveFingerHint();	

		// advance the tutorial
		Advance();		
	}

	private void FocusOnNode(){
		// find and spotlight the tutorial node
		goNode = GameObject.Find("DecoNode_Dojo_WallItem_1");
		// SpotlightObject( goNode );
		ShowFingerHint(goNode, flipX: true);
		
		// add the node to the process list so the user can click it
		AddToProcessList(goNode);
		
		// listen for when the node is clicked
		LgButton button = goNode.GetComponent<LgButton>();
		button.OnProcessed += OnNodeClicked;		
		
		// at this point, also give the user an item for the node they are about to click
		InventoryLogic.Instance.AddItem("WallPoster8", 1);
	}
			
	private void OnNodeClicked(object sender, EventArgs args){
		// stop listening for the node to be clicked
		LgButton button = goNode.GetComponent<LgButton>();
		button.OnProcessed -= OnNodeClicked;

		//Remove button from the clickable list
		RemoveFromProcessList(goNode);

		// clean up	
		RemoveFingerHint();

		// advance the tutorial
		Advance();
	}
			
	private IEnumerator FocusOnDecorationUI(){
		// wait one frame for the UI to appear
		yield return 0;
		
		// find and spotlight the decoration in the user's inventory/UI
		GameObject goEntry = EditDecosUIManager.Instance.GetTutorialEntry();

		//Show finger hint
		ShowFingerHint(goEntry, true, InterfaceAnchors.Bottom, flipX: true);

		AddToProcessList(goEntry);
		
		// listen for when that decoration is actually clicked
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced += OnDecorationPlaced;
	}
			
	private void OnDecorationPlaced(object sender, EventArgs args){
		// stop listening for the decoration clicked callback
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced -= OnDecorationPlaced;

		// clean up
		RemoveFingerHint();
		
		// advance the tutorial
		Advance();
	}

	private IEnumerator FocusOnDecoExitButton(){
		float exitWaitTime = Constants.GetConstant<float>("DecoExitWait");
		yield return new WaitForSeconds(exitWaitTime);

		// clean up notification from the previous step before proceeding
		NotificationUIManager.Instance.CleanupNotification();

		goExitButton = GameObject.Find("DecoExitButton");

		// show finger hint
		ShowFingerHint(goExitButton, true, InterfaceAnchors.BottomRight);

		// show message
		Vector3 vLoc = Constants.GetConstant<Vector3>("DecorationExitPopupLoc");
		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);
		Hashtable option = new Hashtable();

		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, vLoc, useViewPort: false, option: option);

		//permit exit button to be clicked
		AddToProcessList(goExitButton);

		// listen for when the node is clicked
		LgButton button = goExitButton.GetComponent<LgButton>();
		button.OnProcessed += OnDecoModeExit;		
	}

	/// <summary>
	/// Raises the deco mode exit event. Clean up and advance
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDecoModeExit(object sender, EventArgs args){
		LgButton button = goExitButton.GetComponent<LgButton>();
		button.OnProcessed -= OnDecoModeExit;		

		RemoveFingerHint();
		RemovePopup();

		Advance();
	}
}
