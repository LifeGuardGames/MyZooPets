using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver
// Shown when the minigame is over.
//---------------------------------------------------

public abstract class MinigamePopup_GameOver : MinigamePopup {
	// ---------- Pure Abstract -------------------------
	protected abstract int GetScore();								// return the score for this minigame
	protected abstract int GetReward( MinigameRewardTypes eType );	// get the reward for a type
	// --------------------------------------------------
	
	// labels for the stars and xp rewards
	public UILabel labelXP;
	public UILabel labelStars;
	
	// label for the player's score
	public UILabel labelScore;
	
	// icons for the stars and xp rewards -- we spawn stuff from these
	public GameObject goIconStars;
	public GameObject goIconXP;
	
	// the buttons for the game over screen don't come up until the appropriate time
	public PositionTweenToggle tweenButtons;
	private bool bCheckToShowButtons = false;
	
	public int nFreebie; // used for testing purposes
	
	//---------------------------------------------------
	// _OnUpdate()
	//---------------------------------------------------	
	protected override void _OnUpdate() {
		if ( bCheckToShowButtons ) {
			if ( HUDUIManager.Instance.hudAnimator.GetDisplayValue( HUDElementType.Points ) == DataManager.Instance.GameData.Stats.Points &&
					HUDUIManager.Instance.hudAnimator.GetDisplayValue( HUDElementType.Stars ) == DataManager.Instance.GameData.Stats.Stars ) {
				
				bCheckToShowButtons = false;
				
				// show the buttons
				tweenButtons.Show();
			}
		}
	}
	
	//---------------------------------------------------
	// _OnShow()
	//---------------------------------------------------	
	protected override void _OnShow() {
		// set an override for the hud animation modifier because we don't want to spam animations
		//hudAnimator.SetModifierOverride( .00001f );
		
		// the game over screen is showing, so we need to check to show the buttons now too
		bCheckToShowButtons = true;
		
		// the game over screen is showing, so we need to set our stats properly
		
		// set the score
		int nScore = GetScore();
		string strScore = StringUtils.FormatNumber( nScore );
		labelScore.text = strScore;
		
		// set the stars and XP earned
		int nRewardXP = GetReward( MinigameRewardTypes.XP ) + nFreebie;
		int nRewardMoney = GetReward( MinigameRewardTypes.Money ) + nFreebie;
		
		string strXP = StringUtils.FormatNumber( nRewardXP );
		string strMoney = StringUtils.FormatNumber( nRewardMoney );
		
		labelXP.text = strXP;
		labelStars.text = strMoney;
		
		// actually award the money and xp
		Vector3 vPosXP = LgNGUITools.GetScreenPosition( goIconXP );
		Vector3 vPosMoney = LgNGUITools.GetScreenPosition( goIconStars );

		vPosXP = CameraManager.Instance.TransformAnchorPosition( vPosXP, InterfaceAnchors.Center, InterfaceAnchors.Top );
		vPosMoney = CameraManager.Instance.TransformAnchorPosition( vPosMoney, InterfaceAnchors.Center, InterfaceAnchors.Top );
		
		// award the actual xp and money
		StatsController.Instance.ChangeStats(nRewardXP, vPosXP, nRewardMoney, vPosMoney, 0, Vector3.zero, 0, Vector3.zero, true, true, false);
	}
	
	//---------------------------------------------------
	// _OnHide()
	//---------------------------------------------------	
	protected override void _OnHide() {
		// make sure to not check to show the buttons
		bCheckToShowButtons = false;
		tweenButtons.Hide();
	}
}
