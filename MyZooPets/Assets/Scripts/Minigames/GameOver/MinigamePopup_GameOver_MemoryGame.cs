using UnityEngine;
using System.Collections;

public class MinigamePopup_GameOver_MemoryGame : MinigamePopup_GameOver{
	protected override int GetReward(MinigameRewardTypes eType){
		return MemoryGameManager.Instance.GetReward(eType);
	}

	protected override int GetScore(){
		return MemoryGameManager.Instance.GetScore();
	}

	protected override void RewardBadges(){
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Memory, MemoryGameManager.Instance.GetScore(), true);
	}

	protected override bool CheckAndFlagNewGameAd(){
		bool aux = MemoryGameManager.Instance.IsNewGameAd;
		MemoryGameManager.Instance.IsNewGameAd = false;
		return aux;
	}
}
