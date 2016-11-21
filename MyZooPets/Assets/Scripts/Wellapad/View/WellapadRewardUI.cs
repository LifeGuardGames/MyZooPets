using UnityEngine;

//---------------------------------------------------
// WellapadRewardUI
// Small script on the wellapad reward UI for the
// missions list.  This is basically just the text
// and button for the rewerad.
//---------------------------------------------------
public class WellapadRewardUI : MonoBehaviour {
	// mission ID of this reward
	private string strMissionID;
	
	// button for the reward
	//public WellapadRewardButton buttonReward;
	
	public void Init( string strMissionID ) {
		// cache the task
		this.strMissionID = strMissionID;
		
		// init the button belonging to this reward -- the button code will take care of the rest
		//buttonReward.Init( this.strMissionID );
	}
}
