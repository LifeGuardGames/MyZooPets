using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MicroMixManager : NewMinigameManager<MicroMixManager>{
	public Text titleText;
	public Text speedUpText;
	public Micro debugMicro;
	public GameObject[] backgrounds;
	public MicroMixFinger finger;

	public GameObject monsterParent;
	public GameObject monsterBackground;
	public GameObject monsterBody;
	public ParticleSystem monsterParticle;
	public MicroMixPetAnimationManager petAnim;

	public MicroMixLives lifeController;
	public MicroMixFireworks fireworksController;
	public MicroMixBossTimer timer;

	private Micro currentMicro;
	private Micro[] microList;
	private float maxTimeScale = 1.3f;
	private float timeScaleIncrement = .1f;
	private int difficulty = 1;
	private int winScore = 10;
	//True during transition
	private bool isTransitioning;
	//True if we pause the particle while it is playing
	private bool isParticlePaused;
	private bool isTutorial;

	public bool IsTutorial{
		get{
			return isTutorial;
		}
	}

	private enum MonsterAnimation{
		//Win and lose are for the human playing
		INTRO,
		WIN_FINAL,
		WIN,
		LOSE,
		LOSE_FINAL
	}

	void Awake(){
		minigameKey = "MICRO"; //
		quitGameScene = SceneUtils.BEDROOM;
		ResetScore();
	}

	public void WinMicro(){
		int currentScore = lifeController.AddScore();
		UpdateScore(currentScore);
		//AudioManager.Instance.PlayClip("microWin");	
		if(currentScore % 2 == 0){ //We have completed another 2 minigames, SPEEDUP
			difficulty++;
			if(Time.timeScale < maxTimeScale){
				Time.timeScale += timeScaleIncrement;
			}
			speedUpText.color = Color.white;
			speedUpText.transform.localScale = Vector3.one * .75f;
			LeanTween.scale(speedUpText.rectTransform, Vector3.one, .75f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(OnTweenSpeedUp);
			//AudioManager.Instance.PlayClip("microSpeedUp");
		}
		if(currentScore >= winScore){
			StartCoroutine(TransitionIEnum(MonsterAnimation.WIN_FINAL));
		}
		else{
			StartCoroutine(TransitionIEnum(MonsterAnimation.WIN));
		}
	}

	public void LoseMicro(){
		if(difficulty > 1){
			difficulty--;
		}
		if(Time.timeScale > 1f){
			Time.timeScale -= timeScaleIncrement;
		}
		if(lifeController.LoseLife()){
			StartCoroutine(TransitionIEnum(MonsterAnimation.LOSE_FINAL));
		}
		else{
			StartCoroutine(TransitionIEnum(MonsterAnimation.LOSE));
		}
		//AudioManager.Instance.PlayClip("microLose");	
	}

	public void CompleteTutorial(){
		DataManager.Instance.GameData.MicroMix.MicrosCompleted.Add(currentMicro.Title);
		isTutorial = false;
		currentMicro.StartMicro(difficulty, false);
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

	public void Restart() {
		timer.Reset();
		if(currentMicro != null) {
			currentMicro.CancelMicro();
		}
		StopAllCoroutines();
		isPaused = false;
		_NewGame();
	}

	protected override void _NewGame(){
		StartCoroutine(TransitionIEnum(MonsterAnimation.INTRO));
		Time.timeScale = 1f;
		lifeController.Reset(true);
		difficulty = 1;
	}

	protected override void _PauseGame(){
		if(currentMicro != null){
			currentMicro.Pause();
		}
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
		lifeController.Reset(false);
		StartMicro();
	}

	protected override void _GameOver(){
		//AudioManager.Instance.PlayClip("microOver");	
	}

	// Award the actual xp and money, called when tween is complete
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

	#region Transition

	private IEnumerator TransitionIEnum(MonsterAnimation animState){
		float tweenTime = 1f;
		isTransitioning = true;
		monsterBody.GetComponentInChildren<Animator>().speed = 0;
		lifeController.Show();

		if(currentMicro != null){ //Transition in
			currentMicro.gameObject.SetActive(false);
		}
			
		yield return InTransitionHelper(tweenTime);
	
		monsterBody.GetComponentInChildren<Animator>().speed = 1;
		petAnim.animator.speed = 1;
		switch(animState){
		case MonsterAnimation.INTRO:
			int index = Random.Range(0, petAnim.happyIdleAnimations.Count - 1);
			petAnim.animator.Play(petAnim.happyIdleAnimations[index], 0, 0);
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerIntro", 0, 0);
			break;
		case MonsterAnimation.WIN:
			petAnim.animator.speed = 2;
			petAnim.StartFireBlow();
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerWin", 0, 0);
			break;
		case MonsterAnimation.LOSE_FINAL: //Just fall through cause we don't have anything special
		case MonsterAnimation.LOSE:
			petAnim.animator.Play(petAnim.sadIdleAnimations[Random.Range(0, petAnim.sadIdleAnimations.Count)], 0, 0);
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerLose", 0, 0);
			break;
		case MonsterAnimation.WIN_FINAL:
			petAnim.Flipping();
			monsterBody.GetComponentInChildren<Animator>().Play("PlayerWinFinal", 0, 0);
			break;
		default:
			Debug.LogWarning("Invalid anim state");
			break;
		}
		if(animState == MonsterAnimation.WIN){
			yield return WaitSecondsPause(.7f);
			petAnim.FinishFireBlow();
			yield return WaitSecondsPause(1.3f);
		}
		else{
			yield return WaitSecondsPause(2f);
		}
		if(animState == MonsterAnimation.WIN_FINAL){
			GameOver(); //We have finally won
		}
		else if(animState == MonsterAnimation.LOSE_FINAL){
			GameOver(); //We have finally lost
		}
		else{
			ChangeMicro();
			yield return OutTransitionHelper();

			isTransitioning = false;
			lifeController.Hide();
			StartMicro();
		}
	}

	private IEnumerator InTransitionHelper(float tweenTime){
		float radius = 15f; //Tween body
		float angle = Random.value * Mathf.PI * 2;
		monsterBody.transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
		LeanTween.move(monsterBody, new Vector3(3, -.5f), tweenTime).setEase(LeanTweenType.easeInOutBack);

		float angle2 = Random.value * Mathf.PI * 2;
		petAnim.transform.position = new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;
		LeanTween.move(petAnim.gameObject, new Vector3(-3, -1f), tweenTime).setEase(LeanTweenType.easeInOutBack);


		isParticlePaused = true;
		monsterParticle.Play();

		bool hasBackground = currentMicro != null && currentMicro.Background != -1; //Tween old background
		GameObject currentBackground = null;
		if(hasBackground){
			currentBackground = backgrounds[currentMicro.Background];
			foreach(SpriteRenderer spriteRenderer in currentBackground.GetComponentsInChildren<SpriteRenderer>()){
				LeanTween.alpha(spriteRenderer.gameObject, 0, 1f).setEase(LeanTweenType.easeInOutQuad);
			}
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
		float totalTweenTime = monsterParticle.startLifetime;

		float angle = Random.value * Mathf.PI * 2;
		LeanTween.move(monsterBody, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeInOutBack); //Move out monster

		float angle2 = Random.value * Mathf.PI * 2;
		LeanTween.move(petAnim.gameObject, new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeInOutBack); //Move out monster


		monsterParticle.Stop(); //Stop particles
		isParticlePaused = false;

		yield return WaitSecondsPause(totalTweenTime * 1 / 3); //( (0..1)

		GameObject currentBackground = backgrounds[currentMicro.Background];
		currentBackground.SetActive(true);
		foreach(SpriteRenderer spriteRenderer in currentBackground.GetComponentsInChildren<SpriteRenderer>()){
			spriteRenderer.color = new Color(1, 1, 1, 0);//Reset old background
			LeanTween.alpha(spriteRenderer.gameObject, 1, 1f).setEase(LeanTweenType.easeInOutQuad);
		}
		LeanTween.alpha(monsterBackground, 0, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeInOutQuad); 

		yield return WaitSecondsPause(totalTweenTime * 1 / 3); // (1...2)

		titleText.text = currentMicro.Title; //Bring up next title
		titleText.color = Color.white;
		titleText.rectTransform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(titleText.rectTransform, Vector3.one, totalTweenTime * 1 / 3).setEase(LeanTweenType.easeOutQuad).setOnComplete(OnTweenTitle);

		yield return WaitSecondsPause(totalTweenTime * 1 / 3); // (2...3)
	}

	#endregion

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
		Debug.Log(currentMicro);
		currentMicro.gameObject.SetActive(true);
		if(!DataManager.Instance.GameData.MicroMix.MicrosCompleted.Contains(currentMicro.Title)){
			isTutorial = true;
			currentMicro.StartTutorial();
		}
		else{
			currentMicro.StartMicro(difficulty, true);
		}
	}

	private void OnTweenSpeedUp(){
		StartCoroutine(HideText(speedUpText));
	}

	private void OnTweenTitle(){
		StartCoroutine(HideText(titleText));
	}

	private IEnumerator HideText(Text text){
		yield return WaitSecondsPause(.2f);
		LeanTween.textColor(text.rectTransform, Color.clear, .15f);
	}

	private void ResetScore(){
		rewardXPMultiplier = 1f;
		rewardMoneyMultiplier = 1f;
		rewardShardMultiplier = 1f;
		score = 0;
	}
}
