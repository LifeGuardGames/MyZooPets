using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadRewardUI
// Small script on the wellapad reward UI for the
// missions list.  This is basically just the text
// and button for the rewerad.
//---------------------------------------------------

public class WellapadRewardUI : MonoBehaviour {
	// mission ID of this reward
	private string strMissionID;
	
	// task text
	public UILabel label;
	
	// button for the reward
	public WellapadRewardButton buttonReward;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( string strMissionID ) {
		// cache the task
		this.strMissionID = strMissionID;
		
		// set the label showing what the task entails
		label.text = Localization.Localize( "RewardFire" );	
		
		// init the button belonging to this reward -- the button code will take care of the rest
		buttonReward.Init( this.strMissionID );
	}
}
