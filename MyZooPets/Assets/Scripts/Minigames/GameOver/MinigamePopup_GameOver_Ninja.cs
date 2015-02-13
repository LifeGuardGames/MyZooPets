﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_Ninja
// Game over screen for trigger ninja game.
//---------------------------------------------------

public class MinigamePopup_GameOver_Ninja : MinigamePopup_GameOver {
	protected override int GetReward (MinigameRewardTypes eType) {
		return NinjaManager.Instance.GetReward( eType );
	}

	protected override int GetScore () {
		return NinjaManager.Instance.GetScore();
	}

	protected override void _RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Ninja, NinjaManager.Instance.GetScore(), true);
	}
}
