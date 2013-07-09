using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class CalendarLogic{

    public static int PointIncrement = 250;
    public static int StarIncrement = 0;

    //====================API (use this for generating weeks)=======================

    public static List<CalendarEntry> EmptyWeek(){
        List list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry());
        }
        return list;
    }

    public static List<CalendarEntry> LeaveBlankWeek(){ // for those parts that should remain empty
        List list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry(DosageRecord.LeaveBlank, DosageRecord.LeaveBlank));
        }
        return list;
    }

    // All entries should be DosageRecord.LeaveBlank up to today's first entry (either day time or night time)
    public static List<CalendarEntry> LeaveBlankUntilNowWeek(DateTime now){ // for those parts that should remain empty
        List list = EmptyWeek();
        // assume that DateOfSunday is updated by this point

        // get days passed since last Sunday
        int daysPassed = now.Date.Subtract(DataManager.DateOfSunday.AddDays(7).Date);

        // set all values of entries before today to DosageRecord.Miss
        // (except today's)
        for (int i = 0; i < daysPassed - 1; i++){
            CalendarEntry entry = list[i];
            entry.DayTime = DosageRecord.LeaveBlank;
            entry.NightTime = DosageRecord.LeaveBlank;
        }

        // fill in specifically for today
        if (now.Hour >= 12) {
            list[daysPassed - 1].DayTime = DosageRecord.LeaveBlank;
        }

        return list;
    }

    public static List<CalendarEntry> MissedWeek(){
        List list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry(DosageRecord.Miss, DosageRecord.Miss));
        }
        return list;
    }

    // If dateTime is a Sunday, return dateTime itself. Else, return the DateTime of the next Sunday.
    // only used here, and in DataManager to initialize DataManager.DateOfSunday
    public static DateTime GetDateOfSunday(DateTime dateTime){
        if (dateTime.DayOfWeek == DayOfWeek.Sunday){
            return dateTime;
        }
        else {
            int dayOfWeek = (int) dateTime.DayOfWeek;
            DateTime nextSunday = dateTime.AddDays(7 - dayOfWeek).Date;
            return nextSunday;
        }
    }

    //====================API (use this for the UI)=======================

    public static int GetComboCount(){
        return DataManager.CalendarCombo;
    }

    // call after giving inhaler to pet
    // assume that we can only give an inhaler to the pet if it missed it
    public static void RecordGivingInhaler(){
        RecordGivingInhaler(DateTime.Now);
    }

    // call whenever opening calendar
    public static void CalendarOpened(){
        CalendarOpenedOnDate(DateTime.Now);
    }

    public static bool HasCheckedCalendar{
        get{
            TimeSpan sinceLastPlayed = DateTime.Now.Date.Subtract(DataManager.LastCalendarOpenedTime.Date);
            if (sinceLastPlayed.Days == 0){ // same day
                if (DateTime.Now.Hour < 12){ // morning
                    return lastEntry.OpenedInMorning;
                }
                else { // afternoon
                    return lastEntry.OpenedInAfternoon;
                }
            }
            return false;
        }
    }

    //get today's entry
    // might not be necessary anymore
    // public static bool IsThereMissDosageToday{
    //     get{
    //         bool retVal = false;
    //         if(lastEntry != null) retVal = lastEntry.DayTime.Equals(DosageRecord.Miss) ||
    //         lastEntry.NightTime.Equals(DosageRecord.Miss);
    //         return retVal;
    //     }
    // }

    //===========================================

    private static void RecordGivingInhaler(DateTime now){
        if (now.Hour < 12) {
            lastEntry.DayTime = DosageRecord.Hit;
        }
        else if (now.Hour >= 12) {
            lastEntry.NightTime = DosageRecord.Hit;
        }
    }

    private static void CalendarOpenedOnDate(DateTime now){
        UpdateWeekReference(now);
        FillInMissedEntries(now);

        // todo: restart pulsing on any check marks
        DataManager.LastCalendarOpenedTime = now;
    }

    //********************************************
    // other methods

    //Check if it is a new week. Figure out how many weeks need to be re-generated (1 or 2)
    private static void UpdateWeekReference(DateTime now){
        if(now.Date > DataManager.DateOfSunday){ //today's date is later than Sunday

            // If today's date is later than a week past Sunday (two Sundays), then
            // throw away everything and start anew.
            if(now.Date > DataManager.DateOfSunday.AddDays(7)){
                DataManager.EntriesLastWeek = MissedWeek();
                DataManager.ThisWeek = EmptyWeek();
            }
            // Else, we want to move everything up by one week.
            else {
                //create new list for the new week
                DataManager.EntriesLastWeek = DataManager.ThisWeek;
                DataManager.ThisWeek = EmptyWeek();
            }

            DataManager.DateOfSunday = GetDateOfSunday(now);
        }
    }

    private static void FillInMissedEntries(DateTime now){
        // assume that DateOfSunday is updated by this point

        // days passed since last Sunday
        int daysPassed = now.Date.Subtract(DataManager.DateOfSunday.AddDays(7).Date);

        // replace all the DosageRecord.Null values with DosageRecord.Miss
        // (except today's)
        for (int i = 0; i < daysPassed - 1; i++){
            CalendarEntry entry = DataManager.EntriesThisWeek[i];
            if (entry.DayTime == DosageRecord.Null){
                entry.DayTime = DosageRecord.Miss;
            }
            if (entry.NightTime == DosageRecord.Null){
                entry.NightTime = DosageRecord.Miss;
            }
        }

        // fill in specifically for today
        FillInMissesForToday(DataManager.EntriesThisWeek[daysPassed - 1]);
    }

    private static void FillInMissesForToday(CalendarEntry todaysEntry, DateTime now){
        if (now.Hour >= 12) {
            todaysEntry.DayTime = DosageRecord.Miss;
        }
    }
}
