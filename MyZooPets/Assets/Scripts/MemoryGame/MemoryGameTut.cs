using UnityEngine;
using System;
using System.Collections;

public class MemoryGameTut : MinigameTutorial{
	public static string TUT_KEY = "MEMORY_TUT";
	GameObject tutBoards;				// Gameobject that positions the tutorial boards			
	GameObject memoryCards;     		// memory prefab
	
	// in each case we are going to listen to events that tell us to move along
	protected override void ProcessStep(int nStep){
		switch (nStep) {
		//runs for about 4 sec then starts the game the timer is in the memory game manager
		case 0:
			MemoryGameManager.Instance.proceed += MoveAlong;
			//prompt user to shoot
			memoryCards = (GameObject)Resources.Load ("MemoryTut");
			tutBoards = GameObjectUtils.AddChildWithPositionAndScale (GameObject.Find ("Anchor-Center"), memoryCards);
			break;
		}
	}

	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}
	
	protected override void SetMaxSteps(){
		maxSteps = 1;
	}
	// oce we are done destroy the remaining board and reset for the game
	protected override void _End(bool isFinished){
		GameObject.Destroy(tutBoards);
		MemoryGameManager.Instance.inTutorial = false;
		if(!isFinished){
			MemoryGameManager.Instance.Reset();
		}
	}

	private void MoveAlong(object sender, EventArgs args){
		Advance();
	}
}
