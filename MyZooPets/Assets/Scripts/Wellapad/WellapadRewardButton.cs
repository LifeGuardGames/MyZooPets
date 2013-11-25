using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadRewardButton
// This button is on the Wellapad and lets the user
// claim their reward from completing all tasks in a
// mission.
//---------------------------------------------------

public class WellapadRewardButton : LgButton {
	// mission ID associated with this reward
	private string strMissionID;
	
	// reward icon that changes based on the state of the reward
	public UISprite spriteIcon;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( string strMissionID ) {
		this.strMissionID = strMissionID;	
		
		// set the sprites for this button appropriately on init
		SetSprites();		
		
		// listen for when a task is complete so the UI can react
		WellapadMissionController.Instance.OnTaskUpdated += OnTaskUpdated;		
	}	
	
	//---------------------------------------------------
	// SetSprites()
	// Sets the sprites for this button based on what
	// state the reward is in.
	//---------------------------------------------------		
	private void SetSprites() {
		// get the mission associated with this reward
		Mission mission = WellapadMissionController.Instance.GetMission( strMissionID );
		
		if ( mission != null ) {
			// get status of reward
			RewardStatuses eStatus = mission.RewardStatus;
			
			if ( eStatus == RewardStatuses.Claimed ) {
				// if the reward was claimed, just hide the icon sprite
				NGUITools.SetActive( spriteIcon.gameObject, false );
			}
			else {
				// the reward is either unclaimed or unearned -- show the proper icon	
				string strKey = "Reward" + eStatus;
				string strSprite = Constants.GetConstant<string>( strKey );
				spriteIcon.spriteName = strSprite;
			}
		}
	}
	
	//---------------------------------------------------
	// OnTaskUpdated()
	// Callback for when a task's status gets updated.
	//---------------------------------------------------		
	private void OnTaskUpdated( object sender, TaskUpdatedArgs args ) {
		// if the mission IDs match, update our sprite (maybe)
		if ( args.Mission == strMissionID )
			SetSprites();
	}	
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------		
	protected override void ProcessClick() {
		// first check to make sure that the reward is unclaimed -- if it is, claim that bad boy...they've earned it
		Mission mission = WellapadMissionController.Instance.GetMission( strMissionID );
		
		if ( mission != null && mission.RewardStatus == RewardStatuses.Unclaimed ) {
			// claim the reward
			WellapadMissionController.Instance.ClaimReward( strMissionID );
			
			// update the sprite
			SetSprites();
		}
	}
	
	//---------------------------------------------------
	// OnDestory()
	//---------------------------------------------------		
	void OnDestroy() {
		// stop listening for task completion data
		if ( WellapadMissionController.Instance )
			WellapadMissionController.Instance.OnTaskUpdated -= OnTaskUpdated;
	}	
}
