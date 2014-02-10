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
	private string missionID;
	
	// reward icon that changes based on the state of the reward
	public UISprite spriteIcon;
	
	// the NGUI button script for this button
	public UIImageButton nguiButton;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( string missionID ) {
		// set wellapad sprite object
		// buttonWellapad = GameObject.Find( "WellapadButton" ).GetComponent<UIImageButton>();
			
		this.missionID = missionID;	
		
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
		// state of the actual button (not the image on the button)
		bool bEnabled = true;
		
		// get the mission associated with this reward
		Mission mission = WellapadMissionController.Instance.GetMission( missionID );
		
		if ( mission != null ) {
			// get status of reward
			RewardStatuses eStatus = mission.RewardStatus;
			
			// get blink script on the wellapad button
			// BlinkButton scriptBlink = buttonWellapad.gameObject.GetComponent<BlinkButton>();
			
			if ( eStatus == RewardStatuses.Claimed ) {
				// if the reward was claimed, just hide the icon sprite
				NGUITools.SetActive( spriteIcon.gameObject, false );
				
				// remove the blink script from the gameobject, if it existed
				// if ( scriptBlink )
				// 	Destroy( scriptBlink );
				
				// if the reward is claimed, the button is not enabled
				bEnabled = false;
			}
			else {
				// the reward is either unclaimed or unearned -- show the proper icon	
				string strKey = "Reward" + eStatus;
				string strSprite = Constants.GetConstant<string>( strKey );
				spriteIcon.spriteName = strSprite;
				
				// the button is not enabled if the reward is unearned
				bEnabled = eStatus == RewardStatuses.Unclaimed;
				
				// if the status is unclaimed, add a pulse to the wellapad icon (if it doesn't have one)
				// if ( eStatus == RewardStatuses.Unclaimed && scriptBlink == null ) {
				// 	scriptBlink = buttonWellapad.gameObject.AddComponent<BlinkButton>();
				// 	string strBlink = Constants.GetConstant<string>("Wellapad_BlinkSprite");
				// 	float fBlink = Constants.GetConstant<float>("Wellapad_BlinkTime");
					
				// 	scriptBlink.Init(buttonWellapad, strBlink, fBlink);
				// }
			}
		}
		
		nguiButton.isEnabled = bEnabled;
	}
	
	//---------------------------------------------------
	// OnTaskUpdated()
	// Callback for when a task's status gets updated.
	//---------------------------------------------------		
	private void OnTaskUpdated( object sender, TaskUpdatedArgs args ) {
		// if the mission IDs match, update our sprite (maybe)
		if ( args.Mission == missionID )
			SetSprites();
	}	
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------		
	protected override void ProcessClick() {
		// first check to make sure that the reward is unclaimed -- if it is, claim that bad boy...they've earned it
		Mission mission = WellapadMissionController.Instance.GetMission( missionID );
		
		if ( mission != null && mission.RewardStatus == RewardStatuses.Unclaimed ) {
			// claim the reward
			WellapadMissionController.Instance.ClaimReward( missionID );
			
			// update the sprite
			SetSprites();
		}
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	void OnDestroy() {
		// stop listening for task completion data
		// Jason- Don't need to dereference if event handler is not static
		// if ( WellapadMissionController.Instance )
		// 	WellapadMissionController.Instance.OnTaskUpdated -= OnTaskUpdated;
	}	
}
