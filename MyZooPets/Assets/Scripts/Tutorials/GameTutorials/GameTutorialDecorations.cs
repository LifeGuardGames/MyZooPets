using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameTutorialDecorations : GameTutorial{
	// decoration node for tutorial
	private GameObject decoNode;
	private GameObject decoModeBackButton; //reference to deco mode exit button

	private GameObject shopButton;
	private GameObject storeBackButton;
	
	public GameTutorialDecorations() : base(){	
	}	
			
	protected override void SetMaxSteps(){
		maxSteps = 7;
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
			TutorialManager.Instance.StartCoroutine(ShowWellapad());
			break;
		case 1:
			TutorialManager.Instance.StartCoroutine(FocusOnEditButton());
			break;
		case 2:
			FocusOnNode();
			break;
		case 3:
			TutorialManager.Instance.StartCoroutine(FocusOnStoreButton());
			break;			
		case 4:
			StoreUIManager.OnDecorationItemBought += FocusOnStoreExitButton;
			break;
		case 5:
			TutorialManager.Instance.StartCoroutine(FocusOnDecorationUI());
			break;
		case 6:
			TutorialManager.Instance.StartCoroutine(FocusOnDecoExitButton());
			break;
		}
	}
	
	private IEnumerator ShowWellapad(){
		yield return new WaitForSeconds(1.5f);
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

	/// <summary>
	/// Focuses the on decoration node.
	/// </summary>
	private void FocusOnNode(){
		// find and spotlight the tutorial node
		decoNode = GameObject.Find("DecoNode_Starting_Rug");
		// SpotlightObject( goNode );
		ShowFingerHint(decoNode, flipX: true);
		
		// add the node to the process list so the user can click it
		AddToProcessList(decoNode);
		
		// listen for when the node is clicked
		LgButton button = decoNode.GetComponent<LgButton>();
		button.OnProcessed += OnNodeClicked;		
		
		// at this point, also give the user an item for the node they are about to click
//		InventoryLogic.Instance.AddItem("WallPoster8", 1);
	}

	/// <summary>
	/// Node clicked. Clean up and advance.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnNodeClicked(object sender, EventArgs args){
		// stop listening for the node to be clicked
		LgButton button = decoNode.GetComponent<LgButton>();
		button.OnProcessed -= OnNodeClicked;

		//Remove button from the clickable list
		RemoveFromProcessList(decoNode);

		// clean up	
		RemoveFingerHint();

		// advance the tutorial
		Advance();
	}

	/// <summary>
	/// No deocration items yet, so make the user go to the store to buy some
	/// </summary>
	private IEnumerator FocusOnStoreButton(){
		// wait one frame for the UI to appear
		yield return 0;

		// find the store button
		shopButton = EditDecosUIManager.Instance.GetShopButton();

		//Show finger hint
		ShowFingerHint(shopButton, true, InterfaceAnchors.BottomLeft, flipX: true);

		AddToProcessList(shopButton);

		//listen for when the button is clicked
		LgButton button = shopButton.GetComponent<LgButton>();
		button.OnProcessed += OnStoreEntered;
	}

	/// <summary>
	/// Store entered. Clean up and advance.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnStoreEntered(object sender, EventArgs args){
		//stop listening to shop button
		LgButton button = shopButton.GetComponent<LgButton>();
		button.OnProcessed -= OnStoreEntered;

		StoreUIManager.Instance.DisableTabArea();

		//clean up
		RemoveFromProcessList(shopButton);
		RemoveFingerHint();

		// advance the tutorial
		Advance();
	}

	/// <summary>
	/// After some decoration items have been bought, prompt the user to exit
	/// the store
	/// </summary>
	private void FocusOnStoreExitButton(object sender, EventArgs args){
		StoreUIManager.OnDecorationItemBought -= FocusOnStoreExitButton;
		storeBackButton = StoreUIManager.Instance.GetBackButton();

		ShowFingerHint(storeBackButton, isGUI: true, anchor: InterfaceAnchors.TopLeft, flipX: true);

		//permit exit button to be clicked
		AddToProcessList(storeBackButton);
		
		// listen for when the node is clicked
		LgButton button = storeBackButton.GetComponent<LgButton>();
		button.OnProcessed += OnStoreExit;		
	}

	/// <summary>
	/// Store exit. Clean up and advance.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnStoreExit(object sender, EventArgs args){
		LgButton button = storeBackButton.GetComponent<LgButton>();
		button.OnProcessed -= OnStoreExit;

		StoreUIManager.Instance.EnableTabArea();

		RemoveFromProcessList(storeBackButton);
		RemoveFingerHint();

		Advance();
	}

	/// <summary>
	/// There are some items on the decoration inventory now, so focus on the first
	/// item and prompt the user to use it.
	/// </summary>
	private IEnumerator FocusOnDecorationUI(){
		yield return new WaitForSeconds(1);
		// find and spotlight the decoration in the user's inventory/UI
		GameObject decorationItem = EditDecosUIManager.Instance.GetTutorialEntry();
		
		//Show finger hint
		ShowFingerHint(decorationItem, isGUI: true, anchor: InterfaceAnchors.Bottom, flipX: true);
		
		AddToProcessList(decorationItem);
		
		// listen for when that decoration is actually clicked
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced += OnDecorationPlaced;
	}

	/// <summary>
	/// Decoration has been placed so clean up and advance.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDecorationPlaced(object sender, EventArgs args){
		// stop listening for the decoration clicked callback
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced -= OnDecorationPlaced;

		// clean up
		RemoveFingerHint();
		
		// advance the tutorial
		Advance();
	}

	/// <summary>
	/// End of the tutorial so prompt the user to exit decoration mode
	/// </summary>
	private IEnumerator FocusOnDecoExitButton(){
		float exitWaitTime = Constants.GetConstant<float>("DecoExitWait");
		yield return new WaitForSeconds(exitWaitTime);

		// clean up notification from the previous step before proceeding
		NotificationUIManager.Instance.CleanupNotification();

		decoModeBackButton = GameObject.Find("DecoExitButton");

		// show finger hint
		ShowFingerHint(decoModeBackButton, true, InterfaceAnchors.BottomRight);

		// show message
		Vector3 vLoc = Constants.GetConstant<Vector3>("DecorationExitPopupLoc");
		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);
		Hashtable option = new Hashtable();

		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, vLoc, useViewPort: false, option: option);

		//permit exit button to be clicked
		AddToProcessList(decoModeBackButton);

		// listen for when the node is clicked
		LgButton button = decoModeBackButton.GetComponent<LgButton>();
		button.OnProcessed += OnDecoModeExit;		
	}

	/// <summary>
	/// Raises the deco mode exit event. Clean up and advance
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDecoModeExit(object sender, EventArgs args){
		LgButton button = decoModeBackButton.GetComponent<LgButton>();
		button.OnProcessed -= OnDecoModeExit;		

		RemoveFingerHint();
		RemovePopup();

		Advance();
	}
}
