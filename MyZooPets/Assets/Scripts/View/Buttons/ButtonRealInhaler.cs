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
	
//	/// <summary>
//	/// Shows the cutscene.
//	/// </summary>
//	private void ShowCutscene() {
//		GameObject resourceMovie = Resources.Load("Cutscene_Inhaler") as GameObject;
//		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
//		CutsceneFrames.OnCutsceneDone += CutsceneDone;	
//	}
//	
//    private void CutsceneDone(object sender, EventArgs args){
//		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Cutscene_Inhaler");	
//		CutsceneFrames.OnCutsceneDone -= CutsceneDone;
//		ProcessClick();
//    }	

	//--------------------------------------------------
	// Check if inhaler can be used at the current time. 
	// Open if yes or show notification	
	//--------------------------------------------------
	private void CheckToOpenInhaler(){
		if(PlayPeriodLogic.Instance.CanUseRealInhaler()){
			OpenRealInhaler();
		}else{
			/////// Send Notication ////////
			// The notification is going to differ depending on if the user has completed all tutorials or not
			
			// Assign delegate functions to be passed in hashtable
			PopupNotificationNGUI.HashEntry button1Function = delegate(){};

			//Get next play time
			TimeSpan timeSpan = PlayPeriodLogic.Instance.NextPlayPeriod - LgDateTime.GetTimeNow();
        	int countDownTime = timeSpan.Hours + 1;
			
			// choose message based on the state of tutorials
			string strMessage;
			bool bTutsDone = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
			if ( bTutsDone ) 
				strMessage = String.Format(Localization.Localize("NOTIFICATION_DONT_NEED_INHALER"), countDownTime.ToString());
			else
				strMessage = Localization.Localize("NOTIFICATION_DONT_NEED_INHALER_TUT");
			
			// Populate notification entry table
			Hashtable notificationEntry = new Hashtable();
			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.OneButton);
			notificationEntry.Add(NotificationPopupFields.Message, strMessage );
			// notificationEntry.Add(NotificationPopupFields.Button1Label, Localization.Localize("BACK"));
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
		
			// Place notification entry table in static queue
			NotificationUIManager.Instance.AddToQueue(notificationEntry);			
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
