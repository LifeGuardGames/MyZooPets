using UnityEngine;
using System;

public class MemoryGameTut {
	public static string TUT_KEY = "MEMORY_TUT";
	GameObject tutBoards;				// Gameobject that positions the tutorial boards			
	GameObject memoryCards;     		// memory prefab
	
	// in each case we are going to listen to events that tell us to move along
	public void ProcessStep(int nStep){
		switch (nStep) {
		//runs for about 4 sec then starts the game the timer is in the memory game manager
		case 0:
			MemoryGameManager.Instance.proceed += MoveAlong;
			//prompt user to shoot
			memoryCards = (GameObject)Resources.Load ("MemoryTut");
			tutBoards = GameObjectUtils.AddChildGUI(GameObject.Find("Canvas"), memoryCards);
			break;
		}
	}

	// once we are done destroy the remaining board and reset for the game
	public void _End(bool isFinished){
		GameObject.Destroy(tutBoards);
		MemoryGameManager.Instance.inTutorial = false;
		if(!isFinished){
			MemoryGameManager.Instance.Reset();
		}
	}

	private void MoveAlong(object sender, EventArgs args){
		_End(false);
	}
}
