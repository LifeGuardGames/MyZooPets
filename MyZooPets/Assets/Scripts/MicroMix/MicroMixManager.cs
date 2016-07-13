using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MicroMixManager : NewMinigameManager<MicroMixManager>{
	public Text titleText;
	private float speed;
	private float maxTimeScale = 2f;
	private float timeScaleIncrement = .2f;
	private int won;
	private int lost;
	private int difficulty = 1;
	//Micro[] microsPrefabs
	//Micro currentMicro
	void Awake(){
		// Parent settings
		minigameKey = "MICRO"; //
		quitGameScene = SceneUtils.BEDROOM;
		ResetScore();
	}

	public void WinMicro(){
		won++;
		UpdateScore(won);
		if(won % 2 == 0){ //We have completed another 2 minigames, SPEEDUP
			difficulty++;
			if (Time.timeScale<maxTimeScale) {
				Time.timeScale+=timeScaleIncrement;
			}
			//AudioManager.Instance.PlayClip("microSpeedUp");
		}
		//AudioManager.Instance.PlayClip("microWin");	
	}

	public void LoseMicro(){
		lost++;
		if (difficulty>1){
			difficulty--;
		}
		if (Time.timeScale>1f){
			Time.timeScale-=timeScaleIncrement;
		}
		if(lost >= 3){
			GameOver();
		}
		//AudioManager.Instance.PlayClip("microLose");	
	}

	protected override void _Start(){
		//PauseGame(); //Not sure if this is necessary?
	}

	protected override void _StartTutorial(){
		//Complete tutorial automatically
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add(minigameKey);
		NewGame();
	}

	protected override void _NewGame(){
		StartMicro();
	}

	protected override void _PauseGame(){
		//currentGame.Pause();
	}

	protected override void _ResumeGame(){
		//currentGame.Resume();
	}

	protected override void _ContinueGame(){
		lost = 0;
		StartMicro();
	}

	protected override void _GameOver(){
		//AudioManager.Instance.PlayClip("microOver");	
	}

	protected override void _GameOverReward(){
		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);
		FireCrystalManager.Instance.RewardShards(rewardShardAux);
		//BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.DoctorMatch, NumOfCorrectDiagnose, true);
		//TODO: Implement badges under RunnerGame
	}

	protected override void _QuitGame(){
		//Nothing for now
		Time.timeScale = 1f;
	}

	private void StartMicro(){
		Debug.Log("WE'RE IN");
		//titleText.text=title;
		//LeanTween.textAlpha(titleText.rectTransform,0,1.5f).setEase(LeanTweenType.easeOutQuad);
		//Get a random micro from microsPrefabs
		//Start that micro
		//Set currentMicro
	}

	private void ResetScore(){
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
}
