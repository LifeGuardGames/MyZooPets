using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MicroMixManager : NewMinigameManager<MicroMixManager>{
	public Text titleText;
	public Micro debugMicro;
	public GameObject[] backgrounds;
	public MicroMixFinger finger;
	public GameObject monsterParent;
	public GameObject monsterBackground;
	public GameObject monsterBody;
	public ParticleSystem monsterParticle;
	public MicroMixBossTimer bossTimer;
	public MicroMixLives lifeController;

	private Micro currentMicro;
	private Micro[] microList;
	private float maxTimeScale = 1.3f;
	private float timeScaleIncrement = .1f;
	private int won;
	private int lost;
	private int difficulty = 1;
	private bool isTransitioning;
	//True during transition
	private bool isParticlePaused;
	//True if we pause the particle while it is playing

	public bool IsTutorial{
		get;
		set;
	}

	private enum MonsterAnimation{
		//Win and lose are for the human playing
		INTRO,
		WIN,
		LOSE,
		WIN_FINAL
	}

	void Awake(){
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
		StartCoroutine(TransitionIEnum(MonsterAnimation.WIN));
	}

	public void LoseMicro(){
		if(difficulty > 1){
			difficulty--;
		}
		if(Time.timeScale > 1f){
			Time.timeScale -= timeScaleIncrement;
		}
		if(lifeController.LoseLife()){
			GameOver();
		}
		else{
			StartCoroutine(TransitionIEnum(MonsterAnimation.LOSE));
		}
		//AudioManager.Instance.PlayClip("microLose");	
	}

	public IEnumerator WaitSecondsPause(float time){ //Like wait for seconds, but pauses w/ MicroMixManager
		for(float i = 0; i <= time; i += .1f){
			yield return new WaitForSeconds(.1f);
			while(IsPaused){
				yield return 0;
			}
		}
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
		StartCoroutine(TransitionIEnum(MonsterAnimation.INTRO));
		Time.timeScale = 1f;
		won = 0;
		lost = 0;
		difficulty = 1;
	}

	protected override void _PauseGame(){
		if(currentMicro != null){
			currentMicro.Pause();
		}
		bossTimer.Pause();
		LeanTween.pause(finger.gameObject);
		if(isTransitioning){
			if(currentMicro != null){
				foreach(SpriteRenderer sprRenderer in backgrounds[currentMicro.Background].GetComponentsInChildren<SpriteRenderer>()){
					LeanTween.pause(sprRenderer.gameObject);
				}
			}
				
			LeanTween.pause(monsterBackground);
			LeanTween.pause(monsterBody);
			monsterBody.GetComponentInChildren<Animator>().enabled = false;
			monsterParticle.Pause();
		}
	}

	protected override void _ResumeGame(){
		if(currentMicro != null){
			currentMicro.Resume();
		}
		bossTimer.Resume();
		LeanTween.resume(finger.gameObject);
		if(isTransitioning){
			if(currentMicro != null){
				foreach(SpriteRenderer sprRenderer in backgrounds[currentMicro.Background].GetComponentsInChildren<SpriteRenderer>()){
					LeanTween.resume(sprRenderer.gameObject);
				}
			}
				
			LeanTween.resume(monsterBackground);
			LeanTween.resume(monsterBody);
			monsterBody.GetComponentInChildren<Animator>().enabled = true;
			if(isParticlePaused){
				monsterParticle.Play();
			}
		}
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
		Time.timeScale = 1f;
	}

	private IEnumerator TransitionIEnum(MonsterAnimation monsterAnim){
		float tweenTime = 1f;
		float animTime = 2f;
		isTransitioning = true;
		monsterBody.GetComponentInChildren<Animator>().Play("PlayerWin", 0, 0);
		monsterBody.GetComponentInChildren<Animator>().speed = 0;
		lifeController.Show();

		if(currentMicro != null){ //Transition in
			currentMicro.gameObject.SetActive(false);
		}
			
		yield return InTransitionHelper(tweenTime);
	
		switch(monsterAnim){
		case MonsterAnimation.INTRO:
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerIntro", 0, 0);
			animTime = 129f / 60f;
			break;
		case MonsterAnimation.WIN:
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerLose", 0, 0);
			animTime = 112f / 60f;
			break;
		case MonsterAnimation.LOSE:
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerWin", 0, 0);
			animTime = 112f / 60f;
			break;
		case MonsterAnimation.WIN_FINAL:
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerWinFinal", 0, 0);
			animTime = 105f / 60f;
			break;
		default:
			Debug.LogWarning("Invalid anim state");
			break;
		}
		monsterBody.GetComponentInChildren<Animator>().speed = 1;

		yield return WaitSecondsPause(animTime);

		monsterBody.GetComponentInChildren<Animator>().Play("PlayerWin", 0, 0);
		monsterBody.GetComponentInChildren<Animator>().speed = 0;

		ChangeMicro();

		yield return OutTransitionHelper();

		isTransitioning = false;
		lifeController.Hide();
		StartMicro();
	}

	private IEnumerator InTransitionHelper(float tweenTime){
		float radius = 15f; //Tween body
		float angle = Random.value * Mathf.PI * 2;
		monsterBody.transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
		LeanTween.move(monsterBody, new Vector3(3, -1), tweenTime).setEase(LeanTweenType.easeInOutBack);

		isParticlePaused = true;
		monsterParticle.Play();

		bool hasBackground = currentMicro != null && currentMicro.Background != -1; //Tween old background
		GameObject currentBackground = null;
		if(hasBackground){
			currentBackground = backgrounds[currentMicro.Background];
			LeanTween.alpha(currentBackground, 0, 1f).setEase(LeanTweenType.easeInOutQuad);
		}

		monsterBackground.GetComponent<SpriteRenderer>().color = new Color(.239f, .333f, .454f, 0); //Tween monster background
		LeanTween.alpha(monsterBackground, 1, tweenTime).setEase(LeanTweenType.easeInOutQuad);

		yield return WaitSecondsPause(tweenTime); //Yield

		if(hasBackground){
			foreach(SpriteRenderer spriteRenderer in currentBackground.GetComponentsInChildren<SpriteRenderer>()){
				spriteRenderer.color = Color.white;//Reset old background
			}
			currentBackground.SetActive(false);
		}
	}

	private IEnumerator OutTransitionHelper(){
		float radius = 15f; //Tween body
		float angle = Random.value * Mathf.PI * 2;
		float totalTweenTime = monsterParticle.startLifetime;

		LeanTween.move(monsterBody, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeInOutBack); //Move out monster

		monsterParticle.Stop(); //Stop particles
		isParticlePaused = false;

		yield return WaitSecondsPause(totalTweenTime * 1 / 3); //( (0..1)

		GameObject currentBackground = backgrounds[currentMicro.Background];
		currentBackground.SetActive(true);
		foreach(SpriteRenderer spriteRenderer in currentBackground.GetComponentsInChildren<SpriteRenderer>()){
			spriteRenderer.color = new Color(1, 1, 1, 0);//Reset old background
			LeanTween.alpha(spriteRenderer.gameObject, 1, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeInOutQuad);
		}
		LeanTween.alpha(monsterBackground, 0, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeInOutQuad); 

		yield return WaitSecondsPause(totalTweenTime * 1 / 3); // (1...2)

		titleText.text = currentMicro.Title; //Bring up next title
		titleText.color = Color.white;
		titleText.rectTransform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(titleText.rectTransform, Vector3.one, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeOutQuad).setOnComplete(tweenFinished);

		yield return WaitSecondsPause(totalTweenTime * 1 / 3); // (2...3)
	}

	private void ChangeMicro(){
		if(debugMicro == null){ //Set up transition out
			int index = Random.Range(0, microList.Length);
			currentMicro = microList[index];
		}
		else{
			currentMicro = debugMicro;
		}
	}

	private void StartMicro(){
		currentMicro.gameObject.SetActive(true);
		currentMicro.StartMicro(difficulty);
	}

	private void tweenFinished(){
		StartCoroutine(HideText());
	}

	private IEnumerator HideText(){
		yield return WaitSecondsPause(.2f * Time.timeScale); //This will be constant, regardless of how fast game is
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
