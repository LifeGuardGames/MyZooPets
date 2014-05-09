using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CalendarData 
// Save the data for Calendar. 
// Mutable data
//---------------------------------------------------

public class CalendarData{
    public DateTime NextPlayPeriod {get; set;} //the next time that the user can collect check bonuses
	public string TotalTimeRemainInTicks {get; set;} //serializer doesn't serialize TimeSpan, so it's converted into ticks first


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
    public CalendarData(){
        Init();
    }

    private void Init(){
        NextPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();
		TotalTimeRemainInTicks = "";
    }
}
