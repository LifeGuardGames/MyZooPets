using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CalendarData 
// Save the data for Calendar. 
// Mutable data
//---------------------------------------------------

public class MutableDataCalendar{
    public DateTime NextPlayPeriod {get; set;} //the next time that the user can collect check bonuses
	public string TotalTimeRemainInTicks {get; set;} //serializer doesn't serialize TimeSpan, so it's converted into ticks string first


	/// <summary>
	/// Gets the total time remain.
	/// </summary>
	/// <returns>The total time remain.</returns>
	public TimeSpan GetTotalTimeRemain(){
		return new TimeSpan(long.Parse(TotalTimeRemainInTicks));
	}

	/// <summary>
	/// Sets the total time remain.
	/// </summary>
	/// <param name="timeDifference">Time difference.</param>
	public void SetTotalTimeRemain(TimeSpan timeDifference){
		TotalTimeRemainInTicks = timeDifference.Ticks.ToString();
	}

    //================Initialization============
    public MutableDataCalendar(){
        Init();
    }

	/// <summary>
	/// Versions the check. 
	/// TotalTimeRemainInTicks is introduced in v1.2.7. Need to initialize it to
	/// the correct value. TimeSpan.Zero won't work unless it's a complete new
	/// game
	/// </summary>
	public void VersionCheck(){
		TimeSpan totalTimeRemain = NextPlayPeriod - LgDateTime.GetTimeNow();
		TotalTimeRemainInTicks = totalTimeRemain.Ticks.ToString();
	}

    private void Init(){
        NextPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();
		TotalTimeRemainInTicks = TimeSpan.Zero.Ticks.ToString();
    }
}
