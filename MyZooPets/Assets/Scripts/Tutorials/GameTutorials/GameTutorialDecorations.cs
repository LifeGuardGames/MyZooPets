using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameTutorialDecorations : GameTutorial{

	private GameObject decoModeBackButton; //reference to deco mode exit button
	private GameObject decoFingerHint;
	private GameObject shopButton;
	private GameObject storeBackButton;

	public GameTutorialDecorations() : base(){	
	}	
			
	protected override void SetMaxSteps(){
		maxSteps = 6;
	}
			
	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_DECOS;
	}
			
	protected override void _End(bool isFinished){

		// when the tutorial is done reset the shop button function name here.
		LgButton button = shopButton.GetComponent<LgButton>();
		LgButtonMessage buttonMessage = (LgButtonMessage) button;
		buttonMessage.functionName = "OpenShop";

		// since this is the last tutorial, show a little notification
		string strKey = "TUTS_FINISHED";											// key of text to show
		string spriteName = Constants.GetConstant<string>("Tutorial_Finished");		// image to appear on notification
		string petName = DataManager.Instance.GameData.PetInfo.PetName;

		Hashtable notificationEntry = new Hashtable();
		notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TipWithImage);
		notificationEntry.Add(NotificationPopupFields.Message, String.Format(Localization.Localize(strKey), 
		                                                                     StringUtils.FormatStringPossession(petName)));
		notificationEntry.Add(NotificationPopupFields.SpriteName, spriteName);
		NotificationUIManager.Instance.AddToQueue(notificationEntry);

		WellapadMissionController.Instance.TaskCompleted("Decorate");

		GameObject wellapadButton = (GameObject)GameObject.Find("WellapadButton");
		if(wellapadButton != null){
			ButtonWellapad buttonWellapadScript = wellapadButton.GetComponent<ButtonWellapad>();
			buttonWellapadScript.SetListenersToWellapadMissionController();
		}
		else{
			Debug.LogError("wellapad button can't be found: " + this);
		}
	}
			
	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			TutorialManager.Instance.StartCoroutine(ShowWellapad());
			break;
		case 1:
			TutorialManager.Instance.StartCoroutine(FocusOnEditButton());
			break;
		case 2:
			TutorialManager.Instance.StartCoroutine(FocusOnStoreButton());
			break;			
		case 3:
			TutorialManager.Instance.StartCoroutine(WiggleDecorationBuyButtons());
			StoreUIManager.OnDecorationItemBought += FocusOnStoreExitButton;
			break;
		case 4:
			TutorialManager.Instance.StartCoroutine(FocusOnDecorationUI());
			break;
		case 5:
			TutorialManager.Instance.StartCoroutine(FocusOnDecoExitButton());
			break;
		}
	}


	
	private IEnumerator ShowWellapad(){
		yield return new WaitForSeconds(1.5f);
	
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

		ShowPopup(Tutorial.POPUP_STD, location, option: option);
		
		// find and spotlight the edit button
		GameObject goEditButton = NavigationUIManager.Instance.GetEditDecoButton();
		SpotlightObject(goEditButton, true, InterfaceAnchors.BottomLeft, fingerHint: true,
			fingerHintFlip: true, delay: 0.5f);
		
		// add the button to the process list so the user can click it
		AddToProcessList(goEditButton);
		
		// sign up for a callback for when the button is clicked
		DecoInventoryUIManager.Instance.OnManagerOpen += OnEditDecos;
	}	
	
	/// <summary>
	/// After Edit deco mode has started. clean up and advance to next tutorial step
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnEditDecos(object sender, UIManagerEventArgs args){
		// stop listening for callback
		DecoInventoryUIManager.Instance.OnManagerOpen -= OnEditDecos;

		// clean up
		RemoveSpotlight();
		RemovePopup();
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
		shopButton = DecoInventoryUIManager.Instance.GetShopButton();

		//Show finger hint
		ShowFingerHint(shopButton, true, InterfaceAnchors.BottomLeft, flipX: true);

		AddToProcessList(shopButton);

		//listen for when the button is clicked
		LgButton button = shopButton.GetComponent<LgButton>();
		LgButtonMessage buttonMessage = (LgButtonMessage) button;
		buttonMessage.functionName = "OpenShopForTutorial";
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
	/// Wiggles the decoration buy buttons.
	/// and temporary switch the button click callback to a tutorial specific function
	/// </summary>
	/// <returns>The decoration buy buttons.</returns>
	private IEnumerator WiggleDecorationBuyButtons(){
		yield return new WaitForSeconds(1f);
		
		GameObject itemGrid = StoreUIManager.Instance.grid;
		int count = 0;

		foreach(Transform itemTransform in itemGrid.transform){
			StoreItemEntryUIController storeItemEntryUIController = itemTransform.GetComponent<StoreItemEntryUIController>();

			storeItemEntryUIController.PlayWiggleAnimation();
			storeItemEntryUIController.buttonMessage.target = StoreUIManager.Instance.gameObject;
			storeItemEntryUIController.buttonMessage.functionName = "OnBuyButtonTutorial";

			count++;
			if(count == 4) break; //only wiggle the first four deco
		}
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

		//stop buy buttons from wiggling
		GameObject itemGrid = StoreUIManager.Instance.grid;
		int count = 0;

		foreach(Transform itemTransform in itemGrid.transform){
			StoreItemEntryUIController storeItemEntryUIController = itemTransform.GetComponent<StoreItemEntryUIController>();

			storeItemEntryUIController.StopWiggleAnimation();
			storeItemEntryUIController.buttonMessage.target = StoreUIManager.Instance.gameObject;
			storeItemEntryUIController.buttonMessage.functionName = "OnBuyButton";

			count++;
			if(count == 4) break; 
		}
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
	/// There are some items on the decoration inventory now, so hint at the user
	/// to drag the decoration from the inventory to the drop zone
	/// </summary>
	private IEnumerator FocusOnDecorationUI(){
		yield return new WaitForSeconds(1);

		GameObject tutDecoNode = GameObject.Find("DecoNode_0_Rug");
		Vector3 tutDecoNodePosition = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, 
		                                                             tutDecoNode.transform.position);
		tutDecoNodePosition = CameraManager.Instance.TransformAnchorPosition(tutDecoNodePosition, 
		                                                               InterfaceAnchors.BottomLeft, 
		                                                               InterfaceAnchors.BottomRight);
		tutDecoNodePosition.z = 1;

		GameObject tutDecoItemGameObject = DecoInventoryUIManager.Instance.GetTutorialItem();
		Vector3 tutDecoItemPosition = LgNGUITools.GetScreenPosition(tutDecoItemGameObject, isObjectInUIGrid: true);
		decoFingerHint = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-BottomRight/ExtraBottomRightPanel"),
		                                         (GameObject)Resources.Load("DecoFingerHint"));
		decoFingerHint.transform.localPosition = tutDecoItemPosition;

		Hashtable optional = new Hashtable();
		optional.Add("repeat", 0);
		LeanTween.moveLocal(decoFingerHint, tutDecoNodePosition, 2f, optional);

		AddToProcessList(tutDecoItemGameObject);
	
		//listen to when decoration is drop on decoration zone
		DecoInventoryUIManager.OnDecoDroppedOnTarget += OnDecorationPlaced;
	}

	/// <summary>
	/// Decoration has been placed so clean up and advance.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnDecorationPlaced(object sender, EventArgs args){
		// stop listening for the decoration clicked callback
		DecoInventoryUIManager.OnDecoDroppedOnTarget -= OnDecorationPlaced;

		// clean up
		GameObject.Destroy(decoFingerHint);
		
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
		ShowFingerHint(decoModeBackButton);
		SpotlightObject(decoModeBackButton, true, InterfaceAnchors.BottomLeft, fingerHint: true,
		                fingerHintFlip: true, delay: 0.5f);

		// show message
		Vector3 popupLocation = Constants.GetConstant<Vector3>("DecorationExitPopupLoc");
		string tutKey = GetKey() + "_" + GetStep();
		string tutMessage = Localization.Localize(tutKey);
		Hashtable option = new Hashtable();

		option.Add(TutorialPopupFields.ShrinkBgToFitText, true);
		option.Add(TutorialPopupFields.Message, tutMessage);

		ShowPopup(Tutorial.POPUP_STD, popupLocation, option: option);

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

		// clean up
		RemoveSpotlight();
		RemoveFingerHint();
		RemovePopup();

		Advance();
	}
}
