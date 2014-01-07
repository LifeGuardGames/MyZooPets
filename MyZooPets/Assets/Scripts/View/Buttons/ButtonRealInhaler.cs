using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonRealInahler
// Button class that loads up the real inhaler game.
//---------------------------------------------------

public class ButtonRealInhaler : ButtonChangeScene {
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		//Start tutorial if first time; otherwise, open inhaler game
		//if ( DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Cutscene_Inhaler") == false )
		//	ShowCutscene();
		//else if(TutorialLogic.Instance.FirstTimeRealInhaler)
		//	TutorialUIManager.Instance.StartRealInhalerTutorial();
		//else
			CheckToOpenInhaler();
	}
	
	//---------------------------------------------------
	// ShowCutscene()
	//---------------------------------------------------	
	private void ShowCutscene() {
		GameObject resourceMovie = Resources.Load("Cutscene_Inhaler") as GameObject;
		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
		CutsceneFrames.OnCutsceneDone += CutsceneDone;	
	}
	
    private void CutsceneDone(object sender, EventArgs args){
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Cutscene_Inhaler");	
		CutsceneFrames.OnCutsceneDone -= CutsceneDone;
		ProcessClick();
    }	

	//--------------------------------------------------
	// Check if inhaler can be used at the current time. 
	// Open if yes or show notification	
	//--------------------------------------------------
	private void CheckToOpenInhaler(){
		if(PlayPeriodLogic.Instance.CanUseRealInhaler){
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
				strMessage = StringUtils.Replace(Localization.Localize("NOTIFICATION_DONT_NEED_INHALER"), StringUtils.NUM, countDownTime.ToString());
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
	
	//---------------------------------------------------
	// OpenRealInhaler()
	// Also called from tutorial as a callback.
	//---------------------------------------------------
	public void OpenRealInhaler(){
		// use parent
		base.ProcessClick();

		//Hide other UI Objects
		NavigationUIManager.Instance.HidePanel();
		// HUDUIManager.Instance.HidePanel();
	}	
}
