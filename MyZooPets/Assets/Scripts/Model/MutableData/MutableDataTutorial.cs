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
	
    //================Initialization============
    public MutableDataTutorial(){
        Init();
    }

    private void Init(){
		ListPlayed = new List<string>();
    }
}
