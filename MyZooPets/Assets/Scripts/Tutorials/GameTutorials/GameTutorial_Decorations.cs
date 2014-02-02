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
		nMaxSteps = 3;
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
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		switch ( nStep ) {
			case 0:
				TutorialManager.Instance.StartCoroutine( FocusOnEditButton() );
				break;
		case 1:
				FocusOnNode();
				break;
		case 2:
				TutorialManager.Instance.StartCoroutine( FocusOnDecorationUI() );
				break;			
		}
	}
	
	//---------------------------------------------------
	// FocusOnEditButton()
	// This part of the tutorial highlights the edit
	// decos button so that the user presses it.
	//---------------------------------------------------		
	private IEnumerator FocusOnEditButton() {
		// wait a brief moment
		float fWait = Constants.GetConstant<float>( "DecoIntroWait" );
		yield return new WaitForSeconds( fWait );
		
		// find and spotlight the edit button
		GameObject goEditButton = NavigationUIManager.Instance.GetEditDecoButton();
		SpotlightObject( goEditButton, true, InterfaceAnchors.BottomLeft );
		
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
		
		// advance the tutorial
		Advance();		
	}
	
	//---------------------------------------------------
	// FocusOnNode()
	//---------------------------------------------------		
	private void FocusOnNode() {
		// find and spotlight the tutorial node
		goNode = GameObject.Find( "DecoNode_Dojo_WallItem_1" );
		SpotlightObject( goNode );
		
		// add the node to the process list so the user can click it
		AddToProcessList( goNode );
		
		// listen for when the node is clicked
		LgButton button = goNode.GetComponent<LgButton>();
		button.OnProcessed += OnNodeClicked;		
		
		// at this point, also give the user an item for the node they are about to click
		InventoryLogic.Instance.AddItem( "WallPoster1", 1 );
	}
	
	//---------------------------------------------------
	// OnNodeClicked()
	//---------------------------------------------------		
	private void OnNodeClicked( object sender, EventArgs args ) {
		// stop listening for the node to be clicked
		LgButton button = goNode.GetComponent<LgButton>();
		button.OnProcessed -= OnNodeClicked;
		
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
		SpotlightObject( goEntry, true, InterfaceAnchors.Bottom, "TutorialSpotlightDeco" );
		
		// listen for when that decoration is actually clicked
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced += OnDecorationPlaced;
	}
	
	//---------------------------------------------------
	// OnDecorationPlaced()
	//---------------------------------------------------		
	private void OnDecorationPlaced( object sender, EventArgs args ) {
		// stop listening for the decoration clicked callback
		EditDecosUIManager.Instance.GetChooseScript().OnDecoPlaced -= OnDecorationPlaced;
		
		// remove the spotlight
		RemoveSpotlight();
		
		// advance the tutorial
		Advance();
	}
}
