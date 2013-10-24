using UnityEngine;
using System.Collections;

public class GameTutTest : GameTutorial {
	protected override void SetKey() {
		strKey = "GameTutTest";
	}
	
	protected override void _End( bool bFinished ) {
		Debug.Log("Tut test is ending");
	}
	
	protected override void ProcessStep( int nStep ) {
		
	}
}
