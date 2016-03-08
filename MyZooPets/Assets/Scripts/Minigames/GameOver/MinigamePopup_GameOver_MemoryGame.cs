using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_MemoryGame
// Game over screen for the clinic game.
//---------------------------------------------------
public class MinigamePopup_GameOver_MemoryGame : MinigamePopup_GameOver{
	protected override int GetReward(MinigameRewardTypes eType){
		return MemoryGameManager.Instance.GetReward(eType);
	}	
	protected override int GetScore(){
		return MemoryGameManager.Instance.GetScore();
	}

	protected override void _RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Memory, MemoryGameManager.Instance.GetScore(), true);
	}

	protected override bool CheckAndFlagNewGameAd(){
		bool aux = MemoryGameManager.Instance.IsNewGameAd;
		MemoryGameManager.Instance.IsNewGameAd = false;
		return aux;
	}
}
