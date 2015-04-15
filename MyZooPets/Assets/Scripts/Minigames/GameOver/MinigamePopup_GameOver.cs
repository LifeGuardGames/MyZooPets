﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver
// Shown when the minigame is over.
//---------------------------------------------------
using System;

public abstract class MinigamePopup_GameOver : MinigamePopup{
	// ---------- Pure Abstract -------------------------
	protected abstract int GetScore();								// return the score for this minigame
	protected abstract int GetReward(MinigameRewardTypes eType);	// get the reward for a type
	// --------------------------------------------------
	
	// labels for the stars and xp rewards
	public UILabel labelXP;
	public UILabel labelStars;
	public UILabel labelShards;
	
	// label for the player's score
	public UILabel labelScore;
	
	// icons for the stars and xp rewards -- we spawn stuff from these
	public GameObject goIconStars;
	public GameObject goIconXP;
	
	// the buttons for the game over screen don't come up until the appropriate time
	public PositionTweenToggle tweenButtons;
	public int nFreebie; // used for testing purposes

	void Start(){
		// Attach event to only show the game over buttons when all rewards are rewarded
		RewardManager.OnAllRewardsDone += ShowButtons;
	}

	void OnDestroy(){
		RewardManager.OnAllRewardsDone -= ShowButtons;
	}

	protected override void _OnShow(){
		// the game over screen is showing, so we need to set our stats properly
		
		// set the score
		int score = GetScore();
		string scoreText = StringUtils.FormatNumber(score);
		labelScore.text = scoreText;
		
		// set the stars and XP earned
		int rewardXP = GetReward(MinigameRewardTypes.XP) + nFreebie;
		int rewardMoney = GetReward(MinigameRewardTypes.Money) + nFreebie;
		int rewardShard = GetReward(MinigameRewardTypes.Shard) + nFreebie;

		labelXP.text = StringUtils.FormatNumber(rewardXP);
		labelStars.text = StringUtils.FormatNumber(rewardMoney);
		labelShards.text = StringUtils.FormatNumber(rewardShard);
		
		// actually award the money and xp
		Vector3 vPosXP = LgNGUITools.GetScreenPosition(goIconXP);
		Vector3 vPosMoney = LgNGUITools.GetScreenPosition(goIconStars);

		vPosXP = CameraManager.Instance.TransformAnchorPosition(vPosXP, InterfaceAnchors.Center, InterfaceAnchors.Top);
		vPosMoney = CameraManager.Instance.TransformAnchorPosition(vPosMoney, InterfaceAnchors.Center, InterfaceAnchors.Top);
		
		// award the actual xp and money
		Debug.Log("Reward money and xp");
		StatsController.Instance.ChangeStats(deltaPoints: rewardXP, pointsLoc: vPosXP,
		                                     deltaStars: rewardMoney, starsLoc: vPosMoney,
		                                     animDelay: 0.5f);

		FireCrystalManager.Instance.RewardShards(rewardShard);

		_RewardBadges();
	}

	protected abstract void _RewardBadges();

	protected override void _OnHide(){
		HideButtons();
	}

	public void ShowButtons(object obj, EventArgs e){
		Debug.Log("SHOWING BUTTONS");
		tweenButtons.Show();
	}
	
	public void HideButtons(){
		tweenButtons.Hide();
	}
}
