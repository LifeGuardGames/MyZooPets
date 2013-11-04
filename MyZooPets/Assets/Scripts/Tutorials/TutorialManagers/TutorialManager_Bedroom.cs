using UnityEngine;
using System.Collections;

//---------------------------------------------------
// TutorialManager_Bedroom
// Tutorial manager for the bedroom.  Responsible for
// managing, ordering, instantiating, etc, all tutorials
// that happen in this room.
//---------------------------------------------------

public class TutorialManager_Bedroom : TutorialManager {
	// tutorial consts
	public const string TUT_INTRO = "IntroNotification";
	public const string TUT_INHALER = "FOCUS_INHALER";
	public const string TUT_CALENDAR = "FOCUS_CALENDAR";
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------	
	protected override void _Start() {
		Check();
	}
	
	//---------------------------------------------------
	// _Check()
	//---------------------------------------------------		
	protected override void _Check() {
		Debug.Log("Checking for tutorials");
		bool bIntro = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_INTRO );
		bool bFocusInhaler = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_INHALER );
		bool bFocusCalendar = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TUT_CALENDAR);
		
		if ( !bIntro ) {
			// first, check to see if that initial notification has been shown
			TutorialUIManager.AddStandardTutTip( NotificationPopupType.TipWithImage, Localization.Localize( "TUT_BEDROOM_INTRO" ), "guiPanelStatsHealth", IntroDone, true, true, "Tutorial:Bedroom:Intro" );	
		}
		else if ( !bFocusInhaler )  {
			// next check to see if the focus inhaler tutorial should display
			new GameTutorial_FocusInhaler();
		}
		else if ( !bFocusCalendar ) {
			// next check to see if the focus calendar tutorial should display
			new GameTutorial_FocusCalendar();
		}
	}
	
	//---------------------------------------------------
	// IntroDone()
	// Callback for when the intro notification is clicked.
	//---------------------------------------------------	
	private void IntroDone() {
		// mark the tut as viewed
		DataManager.Instance.GameData.Tutorial.ListPlayed.Add( TUT_INTRO );	
		
		// and check to see what the next tut should be
		Check();
	}
}
