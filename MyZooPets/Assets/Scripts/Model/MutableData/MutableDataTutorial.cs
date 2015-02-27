using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Tutotrial Data
// Save data for tutorial
// Mutable data
//---------------------------------------------------
public class MutableDataTutorial{
    public List<string> ListPlayed {get; set;}	// list of tutorials that have been played	

	/// <summary>
	/// Determines whether tutorial part 1 is done
	/// </summary>
	/// <returns><c>true</c> if tutorial part1 is done; otherwise, <c>false</c>.</returns>
	public bool IsTutorialPart1Done(){
		bool isFlameTutorialDone = ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME);
		return isFlameTutorialDone;
	}

	/// <summary>
	/// Determines whether tutorial part 2 is done.
	/// </summary>
	/// <returns><c>true</c> if tutorial part2 is done; otherwise, <c>false</c>.</returns>
	public bool IsTutorialPart2Done(){
		bool isDecoTutorialDone = ListPlayed.Contains(TutorialManagerBedroom.TUT_DECOS);
		return isDecoTutorialDone;
	}

	/// <summary>
	/// Ares the tutorials finished.
	/// </summary>
	/// <returns><c>true</c>, if all tutorials are finished, <c>false</c> otherwise.</returns>
	public bool AreTutorialsFinished(){
		bool areTutorialsDone = ListPlayed.Contains(TutorialManagerBedroom.TUT_LAST);
		return areTutorialsDone;	
	}

	/// <summary>
	/// Determines whether specific tutorial is finished
	/// </summary>
	/// <param name="tutorialID">Tutorial ID.</param>
	public bool IsTutorialFinished(string tutorialID){
		return ListPlayed.Contains(tutorialID);
	}
	
    //================Initialization============
    public MutableDataTutorial(){
        Init();
    }

    private void Init(){
		ListPlayed = new List<string>();
    }

	/*
	public void VersionCheck(Version currentDataVersion){
		Version version131 = new Version("1.3.1");
		Version version134 = new Version("1.3.4");
		
		if(currentDataVersion < version131){
			//Don't have DGT_TUT key anymore so remove this key
			if(ListPlayed.Contains("DGT_TUT"))
				ListPlayed.Remove("DGT_TUT");
		}
		if(currentDataVersion < version134){
			//remove deco tutorial, so it comes up again
			if(ListPlayed.Contains(TutorialManagerBedroom.TUT_DECOS))
				ListPlayed.Remove(TutorialManagerBedroom.TUT_DECOS);
		}
	}
	*/
}
