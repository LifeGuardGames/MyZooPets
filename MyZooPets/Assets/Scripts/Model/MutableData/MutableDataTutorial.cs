using System;
using System.Collections.Generic;

public class MutableDataTutorial{
    public List<string> ListPlayed {get; set;}	// list of tutorials that have been played
	
	public bool IsFlameTutorialDone(){
		return IsTutorialFinished(TutorialManagerBedroom.TUT_FLAME);
    }
	
	public bool AreBedroomTutorialsFinished(){
		return IsTutorialFinished(TutorialManagerBedroom.TUT_LAST);
	}
	
	public bool IsTutorialFinished(string tutorialID){
		return ListPlayed.Contains(tutorialID);
	}
	
    public MutableDataTutorial(){
        Init();
    }

    private void Init(){
		ListPlayed = new List<string>();
    }

	public void VersionCheck(string currentDataVersion){

	}
}
