using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigamePopup_GameOver_DoctorMatch
// Game over screen for the clinic game.
//---------------------------------------------------

public class MinigamePopup_GameOver_DoctorMatch : MinigamePopup_GameOver{

	protected override int GetReward(MinigameRewardTypes eType){
		return DoctorMatchManager.Instance.GetReward(eType);
	}

	protected override int GetScore(){
		return DoctorMatchManager.Instance.GetScore();
	}	
	
	protected override void _RewardBadges(){
		BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.PatientNumber, DoctorMatchManager.Instance.NumOfCorrectDiagnose, true);
	}
}
