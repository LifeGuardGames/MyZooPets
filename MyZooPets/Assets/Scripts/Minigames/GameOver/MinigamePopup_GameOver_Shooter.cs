﻿using UnityEngine;
using System.Collections;

public class MinigamePopup_GameOver_Shooter :MinigamePopup_GameOver {

	protected override int GetReward(MinigameRewardTypes eType){
		return ShooterGameManager.Instance.GetReward(eType);
	}
	
	protected override int GetScore(){
		return ShooterGameManager.Instance.GetScore();
	}

	protected override void _RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Shooter, ShooterGameManager.Instance.GetScore(),true);
	}
}
