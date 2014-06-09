using UnityEngine;
using System.Collections;

public class GameTutTest : GameTutorial {
	protected override void SetKey() {
		tutorialKey = "GameTutTest";
	}
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		maxSteps = 3;
	}	
	
	protected override void _End( bool bFinished ) {
		Debug.Log("Tut test is ending");
	}
	
	protected override void ProcessStep( int nStep ) {
		
	}
}
