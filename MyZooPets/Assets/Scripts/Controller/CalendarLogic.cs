using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class CalendarLogic{

    public static int PointIncrement = 250;
    public static int StarIncrement = 0;

    private static CalendarEntry todaysEntry; //today's entry
    // todo: remove these
    //====================API (deprecated methods)=======================
    // public static int GetComboCount(){
    //     return 0;
    // }

    // public static bool HasCheckedCalendar{
    //     get {return true;}
    // }

    // public static List<CalendarEntry> GetCalendarEntries(){
    //     return DataManager.EntriesThisWeek;
    // }

    // public static bool IsThereMissDosageToday{
    //     get {return true;}
    // }
    // todo: remove these
    //====================API (deprecated testing methods)=======================
    public static CalendarEntry TodaysEntry {
        get {return todaysEntry;}
        set {todaysEntry = value;}
    }

    public static void CalendarOpenedTest(DateTime now){
        CalendarOpenedOnDate(now);
    }

    public static void RecordGivingInhalerTest(DateTime now){
        RecordGivingInhaler(now);
    }

    //====================API (use this for generating weeks)=======================

    public static List<CalendarEntry> EmptyWeek(){
        List<CalendarEntry> list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry());
        }
        return list;
    }

    public static List<CalendarEntry> LeaveBlankWeek(){ // for those parts that should remain empty
        List<CalendarEntry> list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry(DosageRecord.LeaveBlank, DosageRecord.LeaveBlank));
        }
        return list;
    }

    // All entries should be DosageRecord.LeaveBlank up to today's first entry (either day time or night time)
    public static List<CalendarEntry> LeaveBlankUntilNowWeek(DateTime now){ // for those parts that should remain empty
        List<CalendarEntry> list = EmptyWeek();
        // assume that DateOfSunday is updated by this point

        // get days passed since last Sunday
        TimeSpan timePassed = now.Date.Subtract(DataManager.DateOfSunday.AddDays(-7).Date);
        int daysPassed = timePassed.Days;

        // set all values of entries before today to DosageRecord.Miss
        // (except today's)
        for (int i = 0; i < daysPassed - 1; i++){
            CalendarEntry entry = list[i];
            entry.DayTime = DosageRecord.LeaveBlank;
            entry.NightTime = DosageRecord.LeaveBlank;
        }

        todaysEntry = list[daysPassed - 1];
        // fill in specifically for today
        if (now.Hour >= 12) {
            todaysEntry.DayTime = DosageRecord.LeaveBlank;
        }

        return list;
    }

    public static List<CalendarEntry> MissedWeek(){
        List<CalendarEntry> list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry(DosageRecord.Miss, DosageRecord.Miss));
        }
        return list;
    }

    //====================API=======================

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

    public static bool CanUseRealInhaler{
        get {
            if (DateTime.Now.Hour < 12 && todaysEntry.DayTime == DosageRecord.Null) {
                return true;
            }
            else if (DateTime.Now.Hour >= 12 && todaysEntry.NightTime == DosageRecord.Null ) {
                return true;
            }
            return false;
        }
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

    private static void RecordGivingInhaler(DateTime now){
        if (now.Hour < 12) {
            todaysEntry.DayTime = DosageRecord.Hit;
        }
        else if (now.Hour >= 12) {
            todaysEntry.NightTime = DosageRecord.Hit;
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
                DataManager.EntriesThisWeek = EmptyWeek();
            }
            // Else, we want to move everything up by one week.
            else {
                // fill the rest of the entries with misses
                for (int i = 0; i < 7; i++){
                    CalendarEntry entry = DataManager.EntriesThisWeek[i];
                    if (entry.DayTime == DosageRecord.Null){
                        entry.DayTime = DosageRecord.Miss;
                    }
                    if (entry.NightTime == DosageRecord.Null){
                        entry.NightTime = DosageRecord.Miss;
                    }
                }

                DataManager.EntriesLastWeek = DataManager.EntriesThisWeek;
                //create new list for the new week
                DataManager.EntriesThisWeek = EmptyWeek();
            }

            DataManager.DateOfSunday = GetDateOfSunday(now);
        }
    }

    private static void FillInMissedEntries(DateTime now){
        // assume that DateOfSunday is updated by this point

        // days passed since last Sunday
        TimeSpan timePassed = now.Date.Subtract(DataManager.DateOfSunday.AddDays(-7).Date);
        int daysPassed = timePassed.Days;

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

        // update reference to todaysEntry
        todaysEntry = DataManager.EntriesThisWeek[daysPassed - 1];

        // fill in specifically for today
        FillInMissesForToday(now);

    }

    private static void FillInMissesForToday(DateTime now){
        if (now.Hour >= 12) {
            if (todaysEntry.DayTime == DosageRecord.Null){
                todaysEntry.DayTime = DosageRecord.Miss;
            }
        }
    }
}
