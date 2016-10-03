using System;
using System.Collections.Generic;

//---------------------------------------------------
// Tutotrial Data
// Save data for tutorial
// Mutable data
//---------------------------------------------------
public class MutableDataTutorial{
    public List<string> ListPlayed {get; set;}	// list of tutorials that have been played	
	public DateTime Tutorial1DonePlayPeriod {get; set;}
	public bool RUNNERTutorialVersionCheckDone {get; set;}

	public bool IsFlameTutorialDone(){
		return IsTutorialFinished(TutorialManagerBedroom.TUT_FLAME);
    }
	
	public bool AreBedroomTutorialsFinished(){
		return IsTutorialFinished(TutorialManagerBedroom.TUT_LAST);
	}
	
	public bool IsTutorialFinished(string tutorialID){
		return ListPlayed.Contains(tutorialID);
	}
	
    //================Initialization============
    public MutableDataTutorial(){
        Init();
    }

    private void Init(){
		ListPlayed = new List<string>();
		ListPlayed.Add("RUNNER");
		Tutorial1DonePlayPeriod = DateTime.MinValue;
		RUNNERTutorialVersionCheckDone = false;
    }

	public void VersionCheck(Version currentDataVersion){

	}
}
