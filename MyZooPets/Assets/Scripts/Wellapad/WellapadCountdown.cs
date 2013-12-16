﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadCountdown
// This script is on the Wellapad and, when the user
// has completed all available missions/tasks and the
// "Done" screen is showing, this script will update
// the time remaining until new missions are available.
//---------------------------------------------------

public class WellapadCountdown : MonoBehaviour {
	//	label to update the timer
	public UILabel labelTimer;
	
	// bit of a hack - if this is true, the countdown was counting down
	private bool bCounting = false;
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------
	void Update() {
		// if the player can use their inhaler, there is no countdown, so bail out
		if ( CalendarLogic.Instance.CanUseRealInhaler ) {
			// okay, so the player can use their inhaler...but were we previously counting down?
			if ( bCounting ) {
				// if we were, stop
				bCounting = false;
				
				// and then refresh the wellapad screen
				WellapadUIManager.Instance.RefreshScreen();
			}
			return;
		}
		
		// also bail if the wellpaid isn't open
		if ( WellapadUIManager.Instance.IsOpen() == false )
			return;
		
		// if we make it here, we are counting down
		bCounting = true;
		
		// otherwise the user CAN'T use their inhaler and the wellapad is open, so there is a countdown showing
		DateTime next = CalendarLogic.Instance.NextPlayPeriod;
		DateTime now = DateTime.Now;
		TimeSpan left = next - now;
		
		// format the time remaining
		string strTime = string.Format("{0:D2}:{1:D2}:{2:D2}", left.Hours, left.Minutes, left.Seconds);
		
		// set the label
		string strLabel = Localization.Localize( "WELLAPAD_NO_MISSIONS_2" );
		labelTimer.text = StringUtils.Replace( strLabel, StringUtils.TIME, strTime );
	}
}