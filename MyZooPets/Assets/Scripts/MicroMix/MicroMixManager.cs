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
		StartCoroutine(TransitionIEnum(true));
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
			StartCoroutine(TransitionIEnum(false));
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
		StartCoroutine(TransitionIEnum(null));
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
		Time.timeScale = 1f;
	}

	private IEnumerator TransitionIEnum(bool? success){
		float tweenTime = 1f;
		float animTime = 2f;
		if(currentMicro != null){ //Transition in
			currentMicro.gameObject.SetActive(false);
		}

		yield return InTransitionHelper(tweenTime);
	
		/*if(success.HasValue){ //Animate
			if(success.Value){
				Debug.Log("WON ANIMATION");
			}
			else{
				Debug.Log("LOST ANIMATION");
			}
		}*/

		yield return new WaitForSeconds(animTime);

		if(debugMicro == null){ //Set up transition out
			int index = Random.Range(0, microList.Length);
			currentMicro = microList[index];
		}
		else{
			currentMicro = debugMicro;
		}
		yield return OutTransitionHelper(tweenTime);

		StartMicro();
	}

	private IEnumerator InTransitionHelper(float tweenTime){
		float radius = 15f; //Tween body
		float angle = Random.value * Mathf.PI * 2;
		monsterBody.transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
		LeanTween.move(monsterBody, Vector3.zero, tweenTime).setEase(LeanTweenType.easeInOutBack);

		monsterParticle.Play();

		bool hasBackground = currentMicro != null && currentMicro.Background != -1; //Tween old background
		GameObject currentBackground = null;
		if(hasBackground){
			currentBackground = backgrounds[currentMicro.Background];
			LeanTween.alpha(currentBackground, 0, 1f).setEase(LeanTweenType.easeInOutQuad);
		}

		monsterBackground.GetComponent<SpriteRenderer>().color = new Color(.239f, .333f, .454f, 0); //Tween monster background
		LeanTween.alpha(monsterBackground, 1, tweenTime).setEase(LeanTweenType.easeInOutQuad);

		yield return new WaitForSeconds(tweenTime); //Yield

		if(hasBackground){
			currentBackground.GetComponent<SpriteRenderer>().color = Color.white; //Reset old background
			currentBackground.SetActive(false);
		}
	}

	private IEnumerator OutTransitionHelper(float tweenTime){
		float radius = 15f; //Tween body
		float angle = Random.value * Mathf.PI * 2;
		LeanTween.move(monsterBody, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius, tweenTime).setEase(LeanTweenType.easeInOutBack);

		bool hasBackground = currentMicro.Background != -1;
		GameObject currentBackground = null;
		if(hasBackground){
			currentBackground = backgrounds[currentMicro.Background];
			currentBackground.SetActive(true);
			currentBackground.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);//Reset old background
			LeanTween.alpha(currentBackground, 1, tweenTime).setEase(LeanTweenType.easeInOutQuad);
		}
		LeanTween.alpha(monsterBackground, 0,tweenTime).setEase(LeanTweenType.easeInOutQuad);

		monsterParticle.Stop();
		currentMicro.gameObject.SetActive(true);
		yield return new WaitForSeconds(monsterParticle.startLifetime); //Yield
	}

	private void StartMicro(){
		currentMicro.StartMicro(difficulty);
		titleText.text = currentMicro.Title;
		titleText.color = Color.white;
		titleText.rectTransform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(titleText.rectTransform, Vector3.one, .5f * Time.timeScale).setEase(LeanTweenType.easeOutQuad).setOnComplete(tweenFinished);
	
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
