using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MutableDataPlayPeriod{
	public DateTime LastPlayPeriod {get; set;}		// Last play period that use has signed on
	public DateTime LastTimeUserPlayedGame {get; set;}
	public bool IsFirstPlayPeriodAux {get; set;}
	public DateTime FirstPlayPeriod {get; set;}

	public bool IsDisplayedAppNotification { get; set; }	// Has the app displayed a rate app notification yet

	public MutableDataPlayPeriod(){
		Init();
	}

	private void Init(){
		IsFirstPlayPeriodAux = false;
		FirstPlayPeriod = DateTime.MinValue;
		LastPlayPeriod = DateTime.MinValue;
		LastTimeUserPlayedGame = DateTime.MinValue;

		IsDisplayedAppNotification = false;
	}
}
