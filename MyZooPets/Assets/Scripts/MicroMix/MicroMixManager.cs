using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MicroMixManager : NewMinigameManager<MicroMixManager>{
	public Text titleText;
	public Micro[] microList;
	public Micro currentMicro;
	private float speed;
	private float maxTimeScale = 2f;
	private float timeScaleIncrement = .2f;
	private int won;
	private int lost;
	private int difficulty = 1;

	public Micro Current{
		get{
			return currentMicro;
		}
	}

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
			if(Time.timeScale < maxTimeScale){
				Time.timeScale += timeScaleIncrement;
			}
			//AudioManager.Instance.PlayClip("microSpeedUp");
		}
		//AudioManager.Instance.PlayClip("microWin");	
		StartMicro();

	}

	public void LoseMicro(){
		lost++;
		if(difficulty > 1){
			difficulty--;
		}
		if(Time.timeScale > 1f){
			Time.timeScale -= timeScaleIncrement;
		}
		if(lost >= 3){
			GameOver();
		} else {
			StartMicro();
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
		int index = Random.Range(0,microList.Length);
		currentMicro=microList[index];
		currentMicro.StartMicro(difficulty);
		titleText.text=currentMicro.Title;
		titleText.color=Color.white;
		//LeanTween.l(titleText.rectTransform,0,1.5f).setEase(LeanTweenType.easeOutQuad);
		titleText.rectTransform.localScale=Vector3.one*1.5f;
		LeanTween.scale(titleText.rectTransform,Vector3.one,.5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(tweenFinished);
	}
	private void tweenFinished(){
		StartCoroutine(HideText());
	}
	private IEnumerator HideText(){
		yield return new WaitForSeconds(.2f);
		titleText.color=Color.clear;
	}
	private void ResetScore(){
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
	void OnGUI(){
		GUI.Box(new Rect(100,0,100,100),won.ToString()+":"+lost.ToString()+":"+Time.timeScale);
	}
}
