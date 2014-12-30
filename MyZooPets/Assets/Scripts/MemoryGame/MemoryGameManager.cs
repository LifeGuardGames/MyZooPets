﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Memory game manager.
/// The scoring would be done in a count down fashion, so your final score is the score
/// that you get when you complete all the matches.
/// </summary>
public class MemoryGameManager : MinigameManager<MemoryGameManager> {

	public MemoryBoardController boardController;

	public int startScoreValue = 1000;
	public int scoreDecrementValue = 10;
	public int scoreDecrementTimer = 2;

	private int cardsCount;
	private MemoryCard flip1 = null;
	private MemoryCard flip2 = null;
	private bool pauseDelayActive = false;
	private float cardDelayTimer = 0.8f;

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	#region Overridden Functions
	protected override void _Start(){
	}

	protected override void _OnDestroy(){

	}

	protected override string GetMinigameKey(){
		return "MemoryGame";
	}
	
	protected override bool IsTutorialOn(){
		return false;	//TODO Change
	}
	
	protected override bool HasCutscene(){
		return false;	//TODO Change
	}

	protected override void _NewGame(){
		flip1 = null;
		flip2 = null;

		cardsCount = MemoryBoardController.ROW_COUNT * MemoryBoardController.COLUMN_COUNT;

		CancelInvoke("StartScoreCountdown");
		SetScore(startScoreValue);
		InvokeRepeating("StartScoreCountdown", 0f, scoreDecrementTimer);

		ResetBoard();
	}

	protected override void _Update(){
	}

	protected override void _GameOver(){
		// TODO add badges
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
		// Check for negative score
		if(GetScore() - scoreDecrementValue >= 0){
			UpdateScore(scoreDecrementValue * -1);
		}
		else{
			SetScore(0);
		}
	}

	private void ResetBoard(){
		boardController.ResetBoard(DataLoaderMemoryTrigger.GetDataList());
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
			}
			else{
				// Failed, flip back again after delay
				Invoke("UnlockDelayFailure", cardDelayTimer);
			}
		}
	}

	private void UnlockDelaySuccess(){
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

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
//			ResetBoard();
//		}
//	}
	#endregion
}
