using UnityEngine;
using System.Collections;

//---------------------------------------------------
// GameTutorial
// Parent class for all game tutorials.  A game tutorial
// is a tutorial that happens outside a minigame, like
// teaching the player about the inhaler, or triggers, etc.
//---------------------------------------------------

public abstract class GameTutorial : Tutorial {
	public GameTutorial() : base() {
		// let the tutorial manager know that this tutorial has been created
		if ( !TutorialManager.Instance ) {
			Debug.Log("Game tutorial being created but no tutorial manager!?");
			return;
		}
		
		// BEFORE we set the tutorial manager variable, save the game
		DataManager.Instance.SaveGameData();
		
		// set the tutorial manager variable
		TutorialManager.Instance.SetTutorial( this );
	}
	
	//---------------------------------------------------
	// End()
	//---------------------------------------------------		
	protected override void End( bool bFinished ) {
		// call super
		base.End( bFinished );
		
		// have the tut manager set right
		TutorialManager.Instance.SetTutorial( null );
		
		// now that the tutorial is over, force a save
		DataManager.Instance.SaveGameData();
	}
}
