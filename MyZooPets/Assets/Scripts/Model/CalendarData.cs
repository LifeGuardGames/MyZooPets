using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class CalendarData{
    //Calendar Data
    [SerializeThis]
    private List<CalendarEntry> entriesThisWeek;
    [SerializeThis]
    private List<CalendarEntry> entriesLastWeek;
    [SerializeThis]
    private DateTime dateOfSunday; // keep track of the last day of the week,
                                          // so we know if we have to clear the calendar
                                          // for a new week or not.
    [SerializeThis]
    private DateTime lastCalendarOpenedTime; //the last time that the user used the calendar
    [SerializeThis]
    private bool isRewardClaimed; //has the check bonuses been collected by the user
    [SerializeThis]
    private DateTime nextPlayPeriod; //the next time that the user can collect check bonuses
                                            //and be punished for missed entries

    //===============Getters & Setters=================
    public List<CalendarEntry> EntriesThisWeek{
        get{return entriesThisWeek;}
        set{entriesThisWeek = value;}
    }
    public List<CalendarEntry> EntriesLastWeek{
        get{return entriesLastWeek;}
        set{entriesLastWeek = value;}
    }
    public DateTime DateOfSunday{
        get{return dateOfSunday;}
        set{dateOfSunday = value;}
    }
    public DateTime LastCalendarOpenedTime{
        get{return lastCalendarOpenedTime;}
        set{lastCalendarOpenedTime = value;}
    }
    public bool IsRewardClaimed{
        get{return isRewardClaimed;}
        set{isRewardClaimed = value;}
    }
    public DateTime NextPlayPeriod{
        get{return nextPlayPeriod;}
        set{nextPlayPeriod = value;}
    }
    public bool UseDummyData{get; set;} //initialize with test data

    //================Initialization============
    public CalendarData(){}

    public void Init(){
        dateOfSunday = CalendarLogic.GetDateOfSunday(DateTime.Now);
        entriesLastWeek = CalendarLogic.LeaveBlankWeek();
        entriesThisWeek = CalendarLogic.LeaveBlankUntilNowWeek(DateTime.Now);
        isRewardClaimed = false;
        nextPlayPeriod = CalendarLogic.CalculateNextPlayPeriod();

        // set to one day before today so that the entry will be generated for the first day
        lastCalendarOpenedTime = DateTime.Today.AddDays(-1);
    }
}
