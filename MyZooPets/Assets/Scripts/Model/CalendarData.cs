using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CalendarData 
// Save the data for Calendar. Mutable data
//---------------------------------------------------

public class CalendarData{
    // public List<CalendarEntry> EntriesThisWeek {get; set;}
    // public List<CalendarEntry> EntriesLastWeek {get; set;}
    // public DateTime DateOfSunday {get; set;} // keep track of the last day of the week,
                                          // so we know if we have to clear the calendar
                                          // for a new week or not.
    // public DateTime LastCalendarOpenedTime {get; set;} //the last time that the user used the calendar
    // public bool IsRewardClaimed {get; set;} //has the check bonuses been collected by the user
    public DateTime NextPlayPeriod {get; set;} //the next time that the user can collect check bonuses

    //================Initialization============
    public CalendarData(){}

    public void Init(){
        // DateOfSunday = PlayPeriodLogic.GetDateOfSunday(LgDateTime.GetTimeNow());

        //initialize this week with sample data to be used for tutorial
        // EntriesThisWeek = PlayPeriodLogic.NullUntilTodayWeek(LgDateTime.GetTimeNow());
        if(LgDateTime.GetTimeNow().Hour < 12){ 
            //next reward time at noon
            NextPlayPeriod = LgDateTime.Today;
        }else{ 
            //next reward time at midnight
            NextPlayPeriod = LgDateTime.Today.AddHours(12);
        }
    }
}
