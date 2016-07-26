using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MicroMixManager : NewMinigameManager<MicroMixManager>{
	public Text titleText;
	public Micro debugMicro;
	public GameObject[] backgrounds;
	public MicroMixFinger finger;
	private Micro currentMicro;
	private Micro[] microList;
	private float maxTimeScale = 1.3f;
	private float timeScaleIncrement = .1f;
	private int won;
	private int lost;
	private int difficulty = 1;

	public bool IsTutorial{
		get;
		set;
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
		}
		else{
			StartMicro();
		}
		//AudioManager.Instance.PlayClip("microLose");	
	}

	protected override void _Start(){
		microList = FindObjectsOfType<Micro>();
		foreach(Micro micro in microList){
			micro.gameObject.SetActive(false);
		}
		//PauseGame(); //Not sure if this is necessary?
	}

	protected override void _StartTutorial(){
		//Complete tutorial automatically
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add(minigameKey);
		NewGame();
	}

	protected override void _NewGame(){
		StartMicro();
		Time.timeScale = 1f;
		won = 0;
		lost = 0;
		difficulty = 1;
	}

	protected override void _PauseGame(){
		Debug.Log("PAAAAUSE");
		currentMicro.Pause();
	}

	protected override void _ResumeGame(){
		currentMicro.Resume();
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
		if(currentMicro != null){
			currentMicro.gameObject.SetActive(false);
			backgrounds[currentMicro.Background].SetActive(false);
		}
		if(debugMicro == null){
			int index = Random.Range(0, microList.Length);
			currentMicro = microList[index];
		}
		else{
			currentMicro = debugMicro;
		}
		currentMicro.gameObject.SetActive(true);
		currentMicro.StartMicro(difficulty);
		titleText.text = currentMicro.Title;
		titleText.color = Color.white;
		titleText.rectTransform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(titleText.rectTransform, Vector3.one, .5f * Time.timeScale).setEase(LeanTweenType.easeOutQuad).setOnComplete(tweenFinished);
		if(currentMicro.Background == 0){
			//if (Random.value>.5f){ This will be moved to each specific minigame that can use day and night
			backgrounds[currentMicro.Background].SetActive(true);
			//} else {
			//	backgrounds[6].SetActive(true);
			//}
		}
		else{
			backgrounds[currentMicro.Background].SetActive(true);
		}
	}

	private void tweenFinished(){
		StartCoroutine(HideText());
	}

	private IEnumerator HideText(){
		yield return new WaitForSeconds(.2f * Time.timeScale); //This will be constant, regardless of how fast game is
		titleText.color = Color.clear;
	}

	private void ResetScore(){
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}

	void OnGUI(){
		GUI.Box(new Rect(100, 0, 100, 100), won.ToString() + ":" + lost.ToString() + ":" + Time.timeScale);
	}
}
