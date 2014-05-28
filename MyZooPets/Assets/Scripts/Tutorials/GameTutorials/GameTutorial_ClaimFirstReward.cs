using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_ClaimFirstReward
// Tutorial for showing the player how to claim a
// reward from the wellapad.
//---------------------------------------------------

public class GameTutorial_ClaimFirstReward : GameTutorial {
	
	public GameTutorial_ClaimFirstReward() : base() {	
	}	
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		maxSteps = 1;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		tutorialKey = TutorialManagerBedroom.TUT_CLAIM_FIRST;
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
				// start by bringing up the wellapad
				WellapadMissionController.Instance.StartCoroutine( OpenWellapad() );
				
				break;
		}
	}
	
	//---------------------------------------------------
	// OpenWellapad()
	//---------------------------------------------------	
	private IEnumerator OpenWellapad() {
		// yield a frame because the wellapad isn't actually instantiated yet
		yield return 0;
		
		// highlight the inhaler task
		WellapadMissionController.Instance.HighlightTask( "DailyInhaler" );	
		
		// add the reward button to list of clickables
		// this is a bit hacky...
		GameObject goReward = GameObject.Find( "2_WellapadTask" );
		while ( goReward == null ) {
			yield return 0;
			goReward = GameObject.Find( "2_WellapadTask" );
		}
		
		WellapadRewardUI scriptReward = goReward.GetComponent<WellapadRewardUI>();
		if ( scriptReward )
			AddToProcessList( scriptReward.buttonReward.gameObject );
		
		// listen for reward claimed callback
		WellapadMissionController.Instance.OnRewardClaimed += OnRewardClaimed;
		
		// now open the wellapad
		WellapadUIManager.Instance.OpenUI();		
	}
	
	//---------------------------------------------------
	// OnRewardClaimed()
	// Callback for when the user claims their first
	// reward.
	//---------------------------------------------------	
	private void OnRewardClaimed( object sender, EventArgs args ) {
		// stop listening to callback
		WellapadMissionController.Instance.OnRewardClaimed -= OnRewardClaimed;
		
		// close the UI
		WellapadUIManager.Instance.CloseUI();
		
		// advance the tutorial
		Advance();
	}
}
