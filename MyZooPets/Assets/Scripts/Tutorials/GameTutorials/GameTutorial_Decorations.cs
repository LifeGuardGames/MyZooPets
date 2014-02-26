using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_Decorations
// Tutorial to explain decoration system.
//---------------------------------------------------

public class GameTutorial_Decorations : GameTutorial {
	// decoration node for tutorial
	private GameObject goNode;
	
	public GameTutorial_Decorations() : base() {	
	}	
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 5;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		strKey = TutorialManager_Bedroom.TUT_DECOS;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected override void _End( bool bFinished ) {
		// since this is the last tutorial, show a little notification
		string strKey = "TUTS_FINISHED";											// key of text to show
		string strImage = Constants.GetConstant<string>("Tutorial_Finished");		// image to appear on notification
		string strAnalytics="";														// analytics tracker

		// show the standard popup
        string petName = DataManager.Instance.GameData.PetInfo.PetName;
		TutorialUIManager.AddStandardTutTip( NotificationPopupType.TipWithImage, 
			String.Format(Localization.Localize(strKey), petName, 
			StringUtils.FormatStringPossession(petName)),
			strImage, null, true, true, strAnalytics );

		GameObject wellapadButton = (GameObject) GameObject.Find("WellapadButton");
		if(wellapadButton != null){
			ButtonWellapad buttonWellapadScript = wellapadButton.GetComponent<ButtonWellapad>();
			buttonWellapadScript.SetListenersToWellapadMissionController();
		}else{
			Debug.LogError("wellapad button can't be found: " + this);
		}

	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		switch ( nStep ) {
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

	//---------------------------------------------------
	// ShowWellapad()
	//---------------------------------------------------		
	private void ShowWellapad() {
		// highlight the fight task
		WellapadMissionController.Instance.HighlightTask("Decorate");
	
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
	private void OnWellapadClosed(object sender, UIManagerEventArgs args) {
		if (args.Opening == false) {
			// wellapad is closing, so stop listening
			WellapadUIManager.Instance.OnManagerOpen -= OnWellapadClosed;
			
			// advance to next step
			Advance();
		}
	}	
	
	//---------------------------------------------------
	// FocusOnEditButton()
	// This part of the tutorial highlights the edit
	// decos button so that the user presses it.
	//---------------------------------------------------		
	private IEnumerator FocusOnEditButton() {
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

		ShowPopup(Tutorial.POPUP_STD, location, option:option);
		
		// find and spotlight the edit button
		GameObject goEditButton = NavigationUIManager.Instance.GetEditDecoButton();
		SpotlightObject(goEditButton, true, InterfaceAnchors.BottomLeft, fingerHint:true,
			fingerHintFlip:true, delay:0.5f);
		
		// add the button to the process list so the user can click it
		AddToProcessList( goEditButton );
		
		// sign up for a callback for when the button is clicked
		EditDecosUIManager.Instance.OnManagerOpen += OnEditDecos;
	}	
	
	//---------------------------------------------------
	// OnEditDecos()
	//---------------------------------------------------		
	private void OnEditDecos( object sender, UIManagerEventArgs args ) {
		// stop listening for callback
		EditDecosUIManager.Instance.OnManagerOpen -= OnEditDecos;

		// clean up
		RemoveSpotlight();
		RemovePopup();
		RemoveFingerHint();	

		// advance the tutorial
		Advance();		
	}
	
	//---------------------------------------------------
	// FocusOnNode()
	//---------------------------------------------------		
	private void FocusOnNode() {
		// find and spotlight the tutorial node
		goNode = GameObject.Find( "DecoNode_Dojo_WallItem_1" );
		// SpotlightObject( goNode );
		ShowFingerHint(goNode, flipX:true);
		
		// add the node to the process list so the user can click it
		AddToProcessList( goNode );
		
		// listen for when the node is clicked
		LgButton button = goNode.GetComponent<LgButton>();
		button.OnProcessed += OnNodeClicked;		
		
		// at this point, also give the user an item for the node they are about to click
		InventoryLogic.Instance.AddItem( "WallPoster8", 1 );
	}
	
	//---------------------------------------------------
	// OnNodeClicked()
	//---------------------------------------------------		
	private void OnNodeClicked( object sender, EventArgs args ) {
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
	
	//---------------------------------------------------
	// FocusOnDecorationUI()
	//---------------------------------------------------		
	private IEnumerator FocusOnDecorationUI() {
		// wait one frame for the UI to appear
		yield return 0;
		
		// find and spotlight the decoration in the user's inventory/UI
		GameObject goEntry = EditDecosUIManager.Instance.GetTutorialEntry();

		//Show finger hint
		ShowFingerHint(goEntry, true, InterfaceAnchors.Bottom, flipX:true);

		AddToProcessList(goEntry);
		
		// listen for when that decoration is actually clicked
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced += OnDecorationPlaced;
	}
	
	//---------------------------------------------------
	// OnDecorationPlaced()
	//---------------------------------------------------		
	private void OnDecorationPlaced( object sender, EventArgs args ) {
		// stop listening for the decoration clicked callback
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced -= OnDecorationPlaced;

		// clean up
		RemoveFingerHint();
		
		// advance the tutorial
		Advance();
	}

	private IEnumerator FocusOnDecoExitButton(){
		float fWait = Constants.GetConstant<float>("DecoExitWait");
		yield return new WaitForSeconds(fWait);

		// clean up notification from the previous step before proceeding
		NotificationUIManager.Instance.CleanupNotification();

		GameObject decoExitButton = GameObject.Find("DecoExitButton");

		// show finger hint
		ShowFingerHint(decoExitButton, true, InterfaceAnchors.BottomRight);

		// show message
		Vector3 vLoc = Constants.GetConstant<Vector3>("DecorationExitPopupLoc");
		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);
		Hashtable option = new Hashtable();

        option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
        option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, vLoc, option:option);

		//permit exit button to be clicked
		AddToProcessList(decoExitButton);

		// listen for when the node is clicked
		LgButton button = decoExitButton.GetComponent<LgButton>();
		button.OnProcessed += OnDecoModeExit;		
	}

	private void OnDecoModeExit(object sender, EventArgs args){
		RemoveFingerHint();
		RemovePopup();

		Advance();
	}
}
