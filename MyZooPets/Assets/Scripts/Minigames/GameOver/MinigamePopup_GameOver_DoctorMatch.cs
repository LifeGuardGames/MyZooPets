using UnityEngine;
using System.Collections;

public class MinigamePopup_GameOver_DoctorMatch : MinigamePopup_GameOver{
	protected override int GetReward(MinigameRewardTypes eType){
		return DoctorMatchManager.Instance.GetReward(eType);
	}

	protected override int GetScore(){
		return DoctorMatchManager.Instance.GetScore();
	}	
	
	protected override void RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.DoctorMatch, DoctorMatchManager.Instance.NumOfCorrectDiagnose, true);
	}

	protected override bool CheckAndFlagNewGameAd(){
		bool aux = DoctorMatchManager.Instance.IsNewGameAd;
		DoctorMatchManager.Instance.IsNewGameAd = false;
		return aux;
	}
}
