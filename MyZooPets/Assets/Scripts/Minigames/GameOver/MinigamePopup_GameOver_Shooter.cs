using UnityEngine;
using System.Collections;

public class MinigamePopup_GameOver_Shooter :MinigamePopup_GameOver {
	protected override int GetReward(MinigameRewardTypes eType){
		return ShooterGameManager.Instance.GetReward(eType);
	}
	
	protected override int GetScore(){
		return ShooterGameManager.Instance.GetScore();
	}

	protected override void RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Shooter, ShooterGameManager.Instance.GetScore(), true);
	}

	protected override bool CheckAndFlagNewGameAd(){
		bool aux = ShooterGameManager.Instance.IsNewGameAd;
		ShooterGameManager.Instance.IsNewGameAd = false;
		return aux;
	}
}
