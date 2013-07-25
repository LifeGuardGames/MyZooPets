using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarLogic : MonoBehaviour{

    private static CalendarEntry todaysEntry; //today's entry
    //===========================Events=======================
    public static event EventHandler<EventArgs> OnCalendarReset; //called when calendar opened or calendar resets
    //========================================================

    //====================API (use this for generating weeks)=======================
    //Generate a week of empty CalendarEntry
    public static List<CalendarEntry> EmptyWeek(){
        List<CalendarEntry> list = new List<CalendarEntry>();
        for (int i = 0; i < 7; i++){
            list.Add(new CalendarEntry());
        }
        return list;
    }

    //Generate a week of CalendarEntry with blank dosage record
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
    //Week in a list. In order from Monday to Sunday
    public List<CalendarEntry> GetCalendarEntriesThisWeek{
        get{return DataManager.EntriesThisWeek;}
    }

    //Last week entries. In order from Monday to Sunday. Possible dosage records are
    //Hit, Miss, and LeaveBlank
    public List<CalendarEntry> GetCalendarEntriesLastWeek{
        get{return DataManager.EntriesLastWeek;}
    }

    //Return the count of all the checks for this week
    public int GreenStampCount{
        get{return DataManager.EntriesThisWeek.FindAll(entry => (entry.DayTime.Equals(DosageRecord.Hit) ||
            entry.NightTime.Equals(DosageRecord.Hit))).Count;}
    }

    //Return the next time the user can collect bonuses
    public DateTime NextPlayPeriod{
        get{return DataManager.NextPlayPeriod;}
    }

    public bool IsRewardClaimed{
        get{return DataManager.IsRewardClaimed;}
        set{DataManager.IsRewardClaimed = value;}
    }

    //Based on the time now return the next reward time
    public static DateTime CalculateNextPlayPeriod(){
        DateTime nextPlayTime;
        if(DateTime.Now.Hour < 12){ //next reward time at noon
            nextPlayTime = DateTime.Today.AddHours(12);
        }else{ //next reward time at midnight
            nextPlayTime = DateTime.Today.AddDays(1);
            // nextPlayTime = new DateTime(2013, 7, 23, 17, 17, 0);
        }
        return nextPlayTime;
    }

    //Give bonus when user collects
    public void ClaimReward(){
		
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(50, 50, 0, 0, Vector3.zero);
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

    public static bool CanUseRealInhaler{
        get {
            bool retVal = false;
            if (DateTime.Now.Hour < 12 && todaysEntry.DayTime == DosageRecord.Null) {
                retVal = true;
            }
            else if (DateTime.Now.Hour >= 12 && todaysEntry.NightTime == DosageRecord.Null ) {
                retVal = true;
            }
            return retVal;
        }
    }

    // call after giving inhaler to pet
    // assume that we can only give an inhaler to the pet if it missed it
    public static void RecordGivingInhaler(){
        RecordGivingInhaler(DateTime.Now);
    }

    // call whenever opening calendar
    public void CalendarOpened(){
        CalendarOpenedOnDate(DateTime.Now);
    }
    //================================================

    private static void RecordGivingInhaler(DateTime now){
        if (now.Hour < 12) {
            todaysEntry.DayTime = DosageRecord.Hit;
        }else if (now.Hour >= 12) {
            todaysEntry.NightTime = DosageRecord.Hit;
        }
    }

    private void CalendarOpenedOnDate(DateTime now){
        UpdateWeekReference(now);
        FillInMissedEntries(now);
        ResetForNextPlayPeriod(now);

        DataManager.LastCalendarOpenedTime = now;

        if(OnCalendarReset != null){
            OnCalendarReset(this, EventArgs.Empty);
        }else{
            Debug.LogError("OnCalendarReset is null");
        }
    }

    //********************************************
    // other methods

    void Update(){
        ResetForNextPlayPeriod(DateTime.Now);
    }

    //Play period is every 12 hr. Reward and punishment renews every play period
    private void ResetForNextPlayPeriod(DateTime now){
        if(now < DataManager.NextPlayPeriod) return; //not next play period yet return
        print("reset");
        //reset green stamps
        for(int i = 0; i < 7; i++){ //new play period so reward can be collected again
            CalendarEntry entry = DataManager.EntriesThisWeek[i];
            if(entry.BonusCollectedDayTime) entry.BonusCollectedDayTime = false;
            if(entry.BonusCollectedNightTime) entry.BonusCollectedNightTime = false;
        }

        //punish for ex stamps
        int punishmentCounter = 0; // max 2
        for(int i = 0;i < 7; i++){
            CalendarEntry entry = DataManager.EntriesThisWeek[i];
            if(entry.DayTime.Equals(DosageRecord.Miss)){
                DataManager.SubtractMood(20);
                DataManager.SubtractHealth(20);
                punishmentCounter++;
            }
            if(entry.NightTime.Equals(DosageRecord.Miss)){
                DataManager.SubtractMood(20);
                DataManager.SubtractHealth(20);
                punishmentCounter++;
            }
            if(punishmentCounter == 2) break;
        }

        //set NextPlayPeriod
        DataManager.NextPlayPeriod = CalculateNextPlayPeriod();


    }

    //Check if it is a new week. Figure out how many weeks need to be re-generated (1 or 2)
    private void UpdateWeekReference(DateTime now){
        if(now.Date > DataManager.DateOfSunday){ //today's date is later than Sunday

            // If today's date is later than a week past Sunday (two Sundays), then
            // throw away everything and start anew.
            if(now.Date > DataManager.DateOfSunday.AddDays(7)){
                DataManager.EntriesLastWeek = MissedWeek();
                DataManager.EntriesThisWeek = EmptyWeek();
            }
            // Else, we want to move everything up by one week. Move this week array to last week
            // array and create an empty array of entries for this week.
            else {
                // if there are any missed entries fill them with misses before moving
                // them to last week array. 
                for (int i = 0; i < 7; i++){
                    CalendarEntry entry = DataManager.EntriesThisWeek[i];
                    if (entry.DayTime == DosageRecord.Null){
                        entry.DayTime = DosageRecord.Miss;
                    }
                    if (entry.NightTime == DosageRecord.Null){
                        entry.NightTime = DosageRecord.Miss;
                    }
                }

                //move this week array to last week array
                DataManager.EntriesLastWeek = DataManager.EntriesThisWeek;
                //create new list for the new week
                DataManager.EntriesThisWeek = EmptyWeek();
            }

            DataManager.DateOfSunday = GetDateOfSunday(now);
        }
    }

    //Fill in the missed entries for this week
    private void FillInMissedEntries(DateTime now){
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

    //Fill in the missed entry for today.
    private void FillInMissesForToday(DateTime now){
        if (now.Hour >= 12) { //PM
            if (todaysEntry.DayTime == DosageRecord.Null){
                todaysEntry.DayTime = DosageRecord.Miss;
            }
        }
    }
}
