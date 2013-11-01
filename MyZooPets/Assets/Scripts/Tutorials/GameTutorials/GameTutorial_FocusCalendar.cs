using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GameTutorial_FocusCalendar
// Tutorial to alert the user to use their calendar.
//---------------------------------------------------

public class GameTutorial_FocusCalendar : GameTutorial {
	// inhaler object
	private GameObject goCalendar = GameObject.Find( "GO_Calendar" );
	
	public GameTutorial_FocusCalendar() : base() {	
	}	
	
	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------		
	protected override void SetMaxSteps() {
		nMaxSteps = 2;
	}
	
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------		
	protected override void SetKey() {
		strKey = TutorialManager_Bedroom.TUT_CALENDAR;
	}
	
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------		
	protected override void _End( bool bFinished ) {
	}
	
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------		
	protected override void ProcessStep( int nStep ) {
		switch ( nStep ) {
			case 0:
				// the start of the focus inhaler tutorial
				FocusCalendar();
				break;
		case 1:
				// user clicked on the calendar, so let the calendar tutorial take over
			
				// remove the spotlight on the calendar
				RemoveSpotlight();
			
				// listen for when the calendar is closed
				CalendarUIManager.Instance.OnManagerOpen += CalendarClosed;
				break;
		}
	}
	
	//---------------------------------------------------
	// FocusCalendar()
	//---------------------------------------------------		
	private void FocusCalendar() {
		// begin listening for when the inhaler is clicked
		LgButton button = goCalendar.GetComponent<LgButton>();
		button.OnProcessed += CalendarClicked;
		
		// the inhaler is the only object that can be clicked
		AddToProcessList( goCalendar );
	
		// spotlight the inhaler
		SpotlightObject( goCalendar );
	}
	
	private void CalendarClosed( object sender, UIManagerEventArgs args ) {
		if ( args.Opening == false ) {
			// calendar is closing, so stop listening
			CalendarUIManager.Instance.OnManagerOpen -= CalendarClosed;
			
			// advance to next step
			Advance();
		}
	}
	
	//---------------------------------------------------
	// CalendarClicked()
	// Callback for when the Calendar object is clicked;
	// this means we need to advance the tutorial.
	//---------------------------------------------------	
	private void CalendarClicked( object sender, EventArgs args ) {
		// stop listening for this event
		LgButton button = (LgButton) sender;
		button.OnProcessed -= CalendarClicked;
		
		// go to the next step
		Advance();
	}
}
