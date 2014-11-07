using UnityEngine;
using System.Collections;

public class MemoryGameManager : MinigameManager<MemoryGameManager> {

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

	}

	protected override void _Update(){
		
	}

	protected override void _GameOver(){

	}

	public override int GetReward(MinigameRewardTypes eType){
		return 0; 		//TODO Change
	}
	#endregion

	#region Game Specific Functions
	private void ResetBoard(){
		
	}


	#endregion
}
