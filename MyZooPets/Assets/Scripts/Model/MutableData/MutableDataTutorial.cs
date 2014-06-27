using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Tutotrial Data
// Save data for tutorial
// Mutable data
//---------------------------------------------------
public class MutableDataTutorial{
    public List<string> ListPlayed {get; set;}	// list of tutorials that have been played	

	public bool IsTutorialFinished(string tutorialID){
		return true;
	}

	public void AddTutorialToFinishList(string tutorialID){

	}

	public bool AreTutorialsFinished() {
		bool bTutsDone = ListPlayed.Contains( TutorialManagerBedroom.TUT_LAST );
		return bTutsDone;	
	}
	
    //================Initialization============
    public MutableDataTutorial(){
        Init();
    }

    private void Init(){
		ListPlayed = new List<string>();
    }
}
