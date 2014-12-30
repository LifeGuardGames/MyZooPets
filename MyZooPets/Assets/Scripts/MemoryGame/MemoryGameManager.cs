using UnityEngine;
using System.Collections;

public class MemoryGameManager : MinigameManager<MemoryGameManager> {

	public MemoryBoardController boardController;

	public int correctScoreValue = 50;
	private int cardsCount;

	private MemoryCard flip1 = null;
	private MemoryCard flip2 = null;
	private bool pauseDelayActive = false;
	private float delayTimer = 0.8f;

	private bool timeBonusActive = true;
	private int timeBonusScoreValue = 100;
	private float timeBonusDuration = 50f;
	private float timeLeft;

	void Awake(){
		quitGameScene = SceneUtils.BEDROOM;
	}

	#region Overridden Functions
	protected override void _Start(){
		timeLeft = timeBonusDuration;
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
		timeBonusActive = true;
		timeLeft = timeBonusDuration;
		ResetBoard();
	}

	protected override void _Update(){
		timeLeft -= Time.deltaTime;
		MemoryGameUIManager memoryUI = ui as MemoryGameUIManager;	// Downcast here
		memoryUI.DisplayTimeLeft(timeLeft);
		if(timeLeft < 0){
			timeBonusActive = false;

		}
	}

	protected override void _GameOver(){
		// TODO add badges
	}

	public override int GetReward(MinigameRewardTypes eType){
		return GetStandardReward(eType);
	}
	#endregion

	#region Game Specific Functions
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
//			Debug.Log("flip1");
			flip1 = card;
		}
		else if(flip2 == null && card != flip1){ // Prevent clicking on self
//			Debug.Log("flip2");
			flip2 = card;

			pauseDelayActive = true;

			if(flip1.TriggerName == flip2.TriggerName){
//				Debug.Log("Success");
				// Match! play scoring sequence after delay
				Invoke("UnlockDelaySuccess", delayTimer);
			}
			else{
//				Debug.Log("Failure");
				// Failed, flip back again after delay
				Invoke("UnlockDelayFailure", delayTimer);
			}
		}
	}

	private void UnlockDelaySuccess(){
		UpdateScore(correctScoreValue);

		// Tell cards to play success state
		flip1.FlipResult(true);
		flip2.FlipResult(true);

		// Display a floaty text of the score increase
		Hashtable floatyOption = new Hashtable();
		floatyOption.Add("prefab", "FloatyTextMemory");
		floatyOption.Add("parent", flip2.transform.parent.gameObject);
		floatyOption.Add("position", flip2.transform.localPosition);
		floatyOption.Add("text", "+" + correctScoreValue);
		FloatyUtil.SpawnFloatyText(floatyOption);

		pauseDelayActive = false;

		// Reset the flips
		flip1 = null;
		flip2 = null;

		cardsCount -= 2;
		if(cardsCount <= 0){
			if(timeBonusActive){
				UpdateScore(timeBonusScoreValue);
			}
			GameOver();
		}
	}

	private void UnlockDelayFailure(){
//		Debug.Log("unlock failure");
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
