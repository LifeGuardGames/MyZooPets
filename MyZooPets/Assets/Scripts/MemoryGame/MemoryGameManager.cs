using UnityEngine;
using System;

/// <summary>
/// The scoring would be done in a count down fashion, so your final score is the score
/// that you get when you complete all the matches.
/// There are also combos if you get 2+ in a row, this adds a multiplier to your score
/// </summary>
public class MemoryGameManager : NewMinigameManager<MemoryGameManager> {
	public MemoryBoardController boardController;
	public EventHandler<EventArgs> proceed;
	public int startScoreValue = 500;
	public int scoreDecrementValue = 10;
	public int scoreDecrementTimer = 2;
	public int comboMultiplier = 25;

	private int cardsCount;
	private MemoryCard flip1 = null;
	private MemoryCard flip2 = null;
	private bool pauseDelayActive = false;
	private float cardDelayTimer = 0.8f;
	private int combo = 0;
	public bool inTutorial = true;
	public GameObject tutButton;

	public MemoryGameUIManager memoryUI;

	private bool isGameActive = false;
	public bool IsGameActive {
		get { return isGameActive; }
	}

	void Awake(){
		minigameKey = "MEMORY";
        quitGameScene = SceneUtils.BEDROOM;
		rewardXPMultiplier = 0.01f;
		rewardMoneyMultiplier = 24;
		rewardShardMultiplier = 84;
		isContinueAllowed = false;	// Disable continue game functionality for this game
	}

	protected override void _Start() {
	}

	protected override void _NewGame() {
		if(IsTutorialOn() && !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(MemoryGameTut.TUT_KEY)) {
			Debug.Log("tut");
			StartTutorial();
			tutButton.SetActive(true);
		}
		else {
			Debug.Log("game");
			isGameActive = true;
			Reset();
        }
	}

	protected override void _PauseGame() {
		isGameActive = false;
	}

	protected override void _ResumeGame() {
		isGameActive = true;
	}

	protected override void _ContinueGame() {
		// Nothing to implement for memory game
	}

	protected override void _GameOver() {
		isGameActive = false;
        //memoryUI.FinishBoard();
	}

	protected override void _GameOverReward() {
		WellapadMissionController.Instance.TaskCompleted("Score" + MinigameKey, Score);

		StatsManager.Instance.ChangeStats(
			xpDelta: rewardXPAux,
			xpPos: GenericMinigameUI.Instance.GetXPPanelPosition(),
			coinsDelta: rewardMoneyAux,
			coinsPos: GenericMinigameUI.Instance.GetCoinPanelPosition(),
			animDelay: 0.5f);
		FireCrystalManager.Instance.RewardShards(rewardShardAux);
		BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Memory, Score, true);

		Analytics.Instance.MemoryGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[MinigameKey]);
		#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "MemoryLeaderBoard");
		#endif
	}

	protected override void _QuitGame() {
	}
	


	protected bool IsTutorialOn(){
		return Constants.GetConstant<bool>("IsMemoryTutorialOn");
	}

	public void Reset(){
		flip1 = null;
		flip2 = null;
		
		cardsCount = MemoryBoardController.ROW_COUNT * MemoryBoardController.COLUMN_COUNT;

		// Reset the combo
		combo = 0;
		memoryUI.SetComboText(combo);

		CancelInvoke("StartScoreCountdown");
		score = startScoreValue;
		InvokeRepeating("StartScoreCountdown", 0f, scoreDecrementTimer);
		//if(!IsTutorialOn() || DataManager.Instance.GameData.Tutorial.IsTutorialFinished(MemoryGameTut.TUT_KEY)){
		ResetBoard();
		//}
	}

	private void ResetBoard(){
		boardController.ResetBoard(DataLoaderMemoryTrigger.GetDataList());
		//memoryUI.StartBoard();
	}

	#region Game Specific Functions
	// InvokeRepeating method from _NewGame()
	private void StartScoreCountdown(){
		if(!IsPaused){
			// Check for negative score
			if(Score - scoreDecrementValue >= 0){
				UpdateScore(scoreDecrementValue * -1);
				memoryUI.UpdateScoreText(score);
			}
			else{
				score = 0;
				memoryUI.UpdateScoreText(score);
			}
		}
	}

	/// <summary>
	/// Determines if flip allowed, check called from MemoryCard.cs itself
	/// </summary>
	/// <returns><c>true</c> if this instance is flip allowed; otherwise, <c>false</c>.</returns>
	public bool IsFlipAllowed(MemoryCard card){
		// Prevent the same card from being clicked
		if(flip1 != null && flip2 == null && card == flip1){
			return false;
		}
		return !pauseDelayActive ;
	}

	/// <summary>
	/// Function that is called whenever a valid flip is done
	/// </summary>
	/// <param name="card">Card</param>
	public void NotifyClicked(MemoryCard card){
		if(flip1 == null){
			flip1 = card;
		}
		else if(flip2 == null && card != flip1){ // Prevent clicking on self
			flip2 = card;

			pauseDelayActive = true;

			if(flip1.TriggerName == flip2.TriggerName){
				// Match! play scoring sequence after delay
				Invoke("UnlockDelaySuccess", cardDelayTimer);

				// Increase the combo
				combo++;
				if(combo >= 1){
					UpdateScore(combo * comboMultiplier);
					memoryUI.UpdateScoreText(score);
				}
			}
			else{
				// Failed, flip back again after delay
				Invoke("UnlockDelayFailure", cardDelayTimer);

				// Reset the combo
				combo = 0;
			}

			// Update the combo text in the UI
			memoryUI.SetComboText(combo);
		}
	}

	private void UnlockDelaySuccess(){
		AudioManager.Instance.PlayClip("memorySuccess");

		// Tell cards to play success state
		flip1.FlipResult(true);
		flip2.FlipResult(true);

		pauseDelayActive = false;

		// Reset the flips
		flip1 = null;
		flip2 = null;

		cardsCount -= 2;

		// Final calculations to the score before game over
		if(cardsCount <= 0){
			CancelInvoke("StartScoreCountdown");
			GameOver();
		}
	}

	private void UnlockDelayFailure(){
		// Tell cards to flip back
		flip1.FlipResult(false);
		flip2.FlipResult(false);

		pauseDelayActive = false;

		// Reset the flips
		flip1 = null;
		flip2 = null;
	}

	public void StartTutorial(){
		MemoryGameTut tut = new MemoryGameTut();
		tut.ProcessStep(0);
		//StartCoroutine (StudyTime ());
	}

	public void MoveOn(){
		tutButton.SetActive(false);
		if(proceed != null)
			proceed(this, EventArgs.Empty);
	}

	//	void OnGUI(){
	//		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
	//			GameOver();
	//		}
	//	}
	#endregion
}
