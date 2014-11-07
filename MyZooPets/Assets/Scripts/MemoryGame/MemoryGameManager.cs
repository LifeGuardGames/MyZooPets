using UnityEngine;
using System.Collections;

public class MemoryGameManager : MinigameManager<MemoryGameManager> {

	public MemoryBoardController boardController;

	private MemoryCard flip1 = null;
	private MemoryCard flip2 = null;
	private bool pauseDelayActive = false;
	private float delayTimer = 0f;

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
		ResetBoard();
	}

	protected override void _Update(){
		
	}

	protected override void _GameOver(){

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
	public bool IsFlipAllowed(){
//		return pauseDelayActive;
		return true;
	}

	/// <summary>
	/// Function that is called whenever a valid flip is done
	/// </summary>
	/// <param name="card">Card.</param>
	public void NotifyClicked(MemoryCard card){
		if(flip1 == null){
			flip1 = card;
		}
		else if(flip2 == null){
			flip2 = card;

			if((card != null) && (flip1.name == flip2.name)){
				// TODO Match!
			}


			// Reset the flips
			flip1 = null;
			flip2 = null;
		}
	}

	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
			ResetBoard();
		}
	}
	#endregion
}
