using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarLogic : Singleton<CalendarLogic>{
    //called when calendar opened or calendar resets
    public static event EventHandler<EventArgs> OnCalendarReset; 

    private CalendarEntry todaysEntry; //today's entry

    //Week in a list. In order from Monday to Sunday
    public List<CalendarEntry> GetCalendarEntriesThisWeek{
        get{return DataManager.Instance.GameData.Calendar.EntriesThisWeek;}
    }

    //Return the count of all the checks for this week
    public int GreenStampCount{
        get{return DataManager.Instance.GameData.Calendar.EntriesThisWeek.FindAll(entry => (entry.DayTime.Equals(DosageRecord.Hit) ||
            entry.NightTime.Equals(DosageRecord.Hit))).Count;}
    }

    //Return the next time the user can collect bonuses
    public DateTime NextPlayPeriod{
        get{return DataManager.Instance.GameData.Calendar.NextPlayPeriod;}
    }

    // public bool IsRewardClaimed{
        // get{return DataManager.Instance.GameData.Calendar.IsRewardClaimed;}
        // set{DataManager.Instance.GameData.Calendar.IsRewardClaimed = value;}
    // } 

    //Check if the user can play the inhaler game
    public bool CanUseRealInhaler{
        get {
            bool retVal = false;
            if (DateTime.Now.Hour < 12 && todaysEntry.DayTime == DosageRecord.Unknown) {
                retVal = true;
            }
            else if (DateTime.Now.Hour >= 12 && todaysEntry.NightTime == DosageRecord.Unknown ) {
                retVal = true;
            }
            return retVal;
        }
    } 

    //Generate a week of empty CalendarEntry
    public static List<CalendarEntry> DosageRecordUnknownWeek(){
        List<CalendarEntry> list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry());
        }
        return list;
    }

    // All entries should be DosageRecord.Null up to today's first entry (either day time or night time)
    public static List<CalendarEntry> NullUntilTodayWeek(DateTime now){ // for those parts that should remain empty
        List<CalendarEntry> list = DosageRecordUnknownWeek();

        // get days passed since last Sunday
        DateTime dateOfSunday = GetDateOfSunday(now);
        TimeSpan timePassed = now.Date.Subtract(dateOfSunday.AddDays(-7).Date);
        int daysPassed = timePassed.Days;

        // set all values of entries before today toDosageRecord.Null 
        // (except today's)
        for (int i = 0; i < daysPassed - 1; i++){
            CalendarEntry entry = list[i];
            entry.DayTime = DosageRecord.Null;
            entry.NightTime = DosageRecord.Null;
        }

        CalendarEntry today = list[daysPassed - 1];
        // fill in specifically for today
        if (now.Hour >= 12) {
            today.DayTime = DosageRecord.Null;
        }

        return list;
    }
	
    //-----------------------------------------------
    // GetTimeFrame()
    // Given a time, will return whether or not that
	// time is considered morning or evening.  All
	// parts of the code should use this to determine
	// what time of day it is.
    //-----------------------------------------------	
	public static TimeFrames GetTimeFrame( DateTime time ) {
		if ( time.Hour > 12 )
			return TimeFrames.Evening;
		else
			return TimeFrames.Morning;
	}

    //-----------------------------------------------
    // CalculateNextPlayPeriod()
    // Based on the time now return the next reward time
    //-----------------------------------------------
    public static DateTime CalculateNextPlayPeriod(){
        DateTime nextPlayTime;
        if(DateTime.Now.Hour < 12){ 
            //next reward time at noon
            nextPlayTime = DateTime.Today.AddHours(12);
        }else{ 
            //next reward time at midnight
            nextPlayTime = DateTime.Today.AddDays(1);
            // nextPlayTime = new DateTime(2013, 7, 23, 17, 17, 0);
        }
        return nextPlayTime;
    }

    //-----------------------------------------------
    // GetDateOfSunday()
    // If dateTime is a Sunday, return dateTime itself. 
    // Else, return the DateTime of the next Sunday.
    // only used here, and in DataManager to initialize DataManager.DateOfSunday
    //-----------------------------------------------
    public static DateTime GetDateOfSunday(DateTime dateTime){
        DateTime dateOfSunday;
        if (dateTime.DayOfWeek == DayOfWeek.Sunday){
            dateOfSunday = dateTime;
        }else {
            int dayOfWeek = (int) dateTime.DayOfWeek;
            DateTime nextSunday = dateTime.AddDays(7 - dayOfWeek).Date;
            dateOfSunday = nextSunday;
        }
        return dateOfSunday;
    }

    //-----------------------------------------------
    // ClaimReward()
    // Give bonus when user collects
    //-----------------------------------------------
    public void ClaimReward(Vector3 screenPos){
		StatsController.Instance.ChangeStats(50, CameraManager.Instance.WorldToScreen( CameraManager.Instance.cameraNGUI, screenPos),
         50, CameraManager.Instance.WorldToScreen( CameraManager.Instance.cameraNGUI, screenPos), 0, Vector3.zero, 0, Vector3.zero);
    }

    //-----------------------------------------------
    // RecrodGivingInhaler()
    // call after giving inhaler to pet
    // assume that we can only give an inhaler to the pet if it missed it
    //-----------------------------------------------
    public void RecordGivingInhaler(){
        DateTime now = DateTime.Now;
        if (now.Hour < 12) {
            todaysEntry.DayTime = DosageRecord.Hit;
        }else if (now.Hour >= 12) {
            todaysEntry.NightTime = DosageRecord.Hit;
        }
    }

    //-----------------------------------------------
    // CalendarOpened()
    // call whenever opening calendar
    //-----------------------------------------------
    public void CalendarOpened(){
        DateTime now = DateTime.Now;
        UpdateCalendar(now);
        DataManager.Instance.GameData.Calendar.LastCalendarOpenedTime = now;

        if(OnCalendarReset != null) 
            OnCalendarReset(this, EventArgs.Empty);
    }


    //-----------------------------------------------
    // ResetWeekAfterTutorialFinish()
    // Reset the week back to blank entries
    //-----------------------------------------------
    // public void ResetWeekAfterTutorialFinish(){
    //     if(D.Assert(OnCalendarReset != null, "OnCalendarReset has no listeners"))
    //         OnCalendarReset(this, EventArgs.Empty);
    // }
    //================================================
    
    void Awake(){
        UpdateCalendar(DateTime.Now);
    }

    //-----------------------------------------------
    // UpdateCalendar()
    // Run a check to see if calendar needs to be updated and reset
    //-----------------------------------------------
    private void UpdateCalendar(DateTime now){
        if(now.Date > DataManager.Instance.GameData.Calendar.DateOfSunday){
            DataManager.Instance.GameData.Calendar.EntriesThisWeek = DosageRecordUnknownWeek(); 

            DataManager.Instance.GameData.Calendar.DateOfSunday = GetDateOfSunday(now);
        }
       // UpdateWeekReference(now);
       FillInMissedEntries(now);
       // ResetForNextPlayPeriod(now);
    }

    //-----------------------------------------------
    // UpdateWeekReference()
    // Check if it is a new week. Figure out how many weeks 
    // need to be re-generated (1 or 2)
    //-----------------------------------------------
    // private void UpdateWeekReference(DateTime now){
    //     if(now.Date > DataManager.Instance.GameData.Calendar.DateOfSunday){ //today's date is later than Sunday

    //         // If today's date is later than a week past Sunday (two Sundays), then
    //         // throw away everything and start anew.
    //         if(now.Date > DataManager.Instance.GameData.Calendar.DateOfSunday.AddDays(7)){
    //             DataManager.Instance.GameData.Calendar.EntriesLastWeek = MissedWeek();
    //             DataManager.Instance.GameData.Calendar.EntriesThisWeek = DosageRecordUnknownWeek();
    //         }
    //         // Else, we want to move everything up by one week. Move this week array to last week
    //         // array and create an empty array of entries for this week.
    //         else {
    //             // if there are any missed entries fill them with misses before moving
    //             // them to last week array. 
    //             for (int i = 0; i < 7; i++){
    //                 CalendarEntry entry = DataManager.Instance.GameData.Calendar.EntriesThisWeek[i];
    //                 if (entry.DayTime == DosageRecord.Unknown){
    //                     entry.DayTime = DosageRecord.Miss;
    //                 }
    //                 if (entry.NightTime == DosageRecord.Unknown){
    //                     entry.NightTime = DosageRecord.Miss;
    //                 }
    //             }

    //             //move this week array to last week array
    //             DataManager.Instance.GameData.Calendar.EntriesLastWeek = DataManager.Instance.GameData.Calendar.EntriesThisWeek;
    //             //create new list for the new week
    //             DataManager.Instance.GameData.Calendar.EntriesThisWeek = DosageRecordUnknownWeek();
    //         }

    //         DataManager.Instance.GameData.Calendar.DateOfSunday = GetDateOfSunday(now);
    //     }
    // }

    //-----------------------------------------------
    // DaysPassedSinceLastSunday()
    //  
    //-----------------------------------------------
    private int DaysPassedSinceLastSunday(DateTime now){
        TimeSpan timePassed = now.Date.Subtract(DataManager.Instance.GameData.Calendar.DateOfSunday.AddDays(-7).Date);
        int daysPassed = timePassed.Days;
        return daysPassed;
    }

    //-----------------------------------------------
    // FillInMissedEntries()
    // Fill in the missed entries for this week
    //-----------------------------------------------
    private void FillInMissedEntries(DateTime now){
        // assume that DateOfSunday is updated by this point

        // days passed since last Sunday
        int daysPassed = DaysPassedSinceLastSunday(now);

        // replace all the DosageRecord.Unknown values with DosageRecord.Miss
        // (except today's)
        for (int i = 0; i < daysPassed - 1; i++){
            CalendarEntry entry = DataManager.Instance.GameData.Calendar.EntriesThisWeek[i];
            if (entry.DayTime == DosageRecord.Unknown){
                entry.DayTime = DosageRecord.Miss;
            }
            if (entry.NightTime == DosageRecord.Unknown){
                entry.NightTime = DosageRecord.Miss;
            }
        }

        // update reference to todaysEntry
        todaysEntry = DataManager.Instance.GameData.Calendar.EntriesThisWeek[daysPassed - 1];

        // fill in specifically for today
        FillInMissesForToday(now);
    }

    //-----------------------------------------------
    // FillInMissesForToday()
    // Fill in the missed entry for today.
    //-----------------------------------------------
    private void FillInMissesForToday(DateTime now){
        if (now.Hour >= 12) { //PM
            if (todaysEntry.DayTime == DosageRecord.Unknown){
                todaysEntry.DayTime = DosageRecord.Miss;
            }
        }
    }

    //-----------------------------------------------
    // ResetForNextPlayPeriod()
    // Play period is every 12 hr. Reward and punishment 
    // renews every play period
    //-----------------------------------------------
    private void ResetForNextPlayPeriod(DateTime now){
        if(now < DataManager.Instance.GameData.Calendar.NextPlayPeriod) return; //not next play period yet return
        //reset green stamps
        for(int i = 0; i < 7; i++){ //new play period so reward can be collected again
            CalendarEntry entry = DataManager.Instance.GameData.Calendar.EntriesThisWeek[i];
            if(entry.BonusCollectedDayTime) entry.BonusCollectedDayTime = false;
            if(entry.BonusCollectedNightTime) entry.BonusCollectedNightTime = false;
        }

        //punish for ex stamps
        int punishmentCounter = 0; // max 2
        for(int i = 0;i < 7; i++){
            CalendarEntry entry = DataManager.Instance.GameData.Calendar.EntriesThisWeek[i];
            if(entry.DayTime.Equals(DosageRecord.Miss)){
                StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, -20, Vector3.zero, -20, Vector3.zero);
                punishmentCounter++;
            }
            if(entry.NightTime.Equals(DosageRecord.Miss)){
                StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, -20, Vector3.zero, -20, Vector3.zero);
                punishmentCounter++;
            }
            if(punishmentCounter == 2) break;
        }

        //set NextPlayPeriod
        DataManager.Instance.GameData.Calendar.NextPlayPeriod = CalculateNextPlayPeriod();
    }

   
}
