using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Tutotrial Data
// Save data for tutorial
// Mutable data
//---------------------------------------------------
public class TutorialData{
    public List<string> ListPlayed {get; set;}	// list of tutorials that have been played	
	
	public bool AreTutorialsFinished() {
		bool bTutsDone = ListPlayed.Contains( TutorialManager_Bedroom.TUT_LAST );
		return bTutsDone;	
	}
	
    //================Initialization============
    public TutorialData(){
        Init();
    }

    private void Init(){
		ListPlayed = new List<string>();
    }
}
