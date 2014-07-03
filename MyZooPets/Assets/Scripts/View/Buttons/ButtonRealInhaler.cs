using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonRealInahler
// Button class that loads up the real inhaler game.
//---------------------------------------------------

public class ButtonRealInhaler : ButtonChangeScene {
	
	/// <summary>
	/// Processes the click.
	/// </summary>
	protected override void ProcessClick() {
		//Start tutorial if first time; otherwise, open inhaler game
		//if ( DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Cutscene_Inhaler") == false )
		//	ShowCutscene();
		//else if(TutorialLogic.Instance.FirstTimeRealInhaler)
		//	TutorialUIManager.Instance.StartRealInhalerTutorial();
		//else
			CheckToOpenInhaler();
	}

	//--------------------------------------------------
	// Check if inhaler can be used at the current time. 
	// Open if yes or show notification	
	//--------------------------------------------------
	private void CheckToOpenInhaler(){
		if(PlayPeriodLogic.Instance.CanUseRealInhaler()){
			OpenRealInhaler();
		}else{
			PlayNotProcessSound();
		}
	}

	/// <summary>
	/// Opens the real inhaler.
	/// </summary>
	public void OpenRealInhaler(){
		// use parent
		base.ProcessClick();
	}	
}
