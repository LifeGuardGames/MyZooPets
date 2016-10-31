using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Memory game manager.
/// The scoring would be done in a count down fashion, so your final score is the score
/// that you get when you complete all the matches.
/// There are also combos if you get 2+ in a row, this adds a multiplier to your score
/// </summary>
public class MemoryGameManager : MinigameManager<MemoryGameManager> {
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
	private bool isPaused = false;
	public bool inTutorial = true;
	public GameObject tutButton;

	private MemoryGameUIManager memoryUI;

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	#region Overridden Functions
	protected override void _Start(){
		MemoryGameManager.OnStateChanged += GameStateChange;
		memoryUI = ui as MemoryGameUIManager;
	}

	protected override void _OnDestroy(){
		MemoryGameManager.OnStateChanged -= GameStateChange;
	}

	protected override string GetMinigameKey(){
		return "Memory";
	}

	protected override void _NewGame(){
		if(IsTutorialOn() && IsTutorialOverride()|| 
		   !DataManager.Instance.GameData.Tutorial.IsTutorialFinished(MemoryGameTut.TUT_KEY)){
			StartTutorial();
			tutButton.SetActive(true);
		}
		else{
			Reset();
		}
	}
	protected override bool IsTutorialOn(){
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
		SetScore(startScoreValue);
		InvokeRepeating("StartScoreCountdown", 0f, scoreDecrementTimer);
		if(!IsTutorialOn()&& !IsTutorialOverride()|| 
		   DataManager.Instance.GameData.Tutorial.IsTutorialFinished(MemoryGameTut.TUT_KEY)){
		ResetBoard();
		}
	}

	private void ResetBoard(){
		boardController.ResetBoard(DataLoaderMemoryTrigger.GetDataList());
		memoryUI.StartBoard();
	}

	protected override void _Update(){
	}

	protected override void _GameOver(){
		memoryUI.FinishBoard();
		WellapadMissionController.Instance.TaskCompleted("Score" + GetMinigameKey(), GetScore());

		Analytics.Instance.MemoryGameData(DataManager.Instance.GameData.HighScore.MinigameHighScore[GetMinigameKey()]);

#if UNITY_IOS
		LeaderBoardManager.Instance.EnterScore((long)GetScore(), "MemoryLeaderBoard");
#endif
	}

	private void GameStateChange(object sender, GameStateArgs args){
		switch(args.GetGameState()){
		case MinigameStates.GameOver:
			break;
		case MinigameStates.Paused:
			isPaused = true;
			break;
		case MinigameStates.Playing:
			isPaused = false;
			break;
		}
	}

	public override int GetReward(MinigameRewardTypes eType){
		return GetStandardReward(eType);
	}
	#endregion

	#region Game Specific Functions
	/// <summary>
	/// Starts the score countdown.
	/// InvokeRepeating method from _NewGame()
	/// </summary>
	private void StartScoreCountdown(){

		if(!isPaused){
			// Check for negative score
			if(GetScore() - scoreDecrementValue >= 0){
				UpdateScore(scoreDecrementValue * -1);
			}
			else{
				SetScore(0);
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

	private void StartTutorial(){
		SetTutorial(new MemoryGameTut());
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
