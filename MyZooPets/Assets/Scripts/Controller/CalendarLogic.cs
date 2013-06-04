using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarLogic : MonoBehaviour {

    private static System.Random rand = new System.Random();
    private static CalendarEntry todaysEntry;
    private static List<CalendarEntry> tempEntries;

    //#region API (use this for the UI)
    public static int GetComboCount(){
        return DataManager.CalendarCombo;
    }

    public static List<CalendarEntry> GetCalendarEntries(){
        return DataManager.Entries;
    }

    // If dateTime is a Sunday, return dateTime itself. Else, return the DateTime of the next Sunday.
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

    // call whenever opening calendar
    public static void CalendarOpened(){
        CalendarOpenedOnDate(DateTime.Today);
    }

    // call after giving inhaler to pet
    public static void RecordAfternoonEntry(){
        RecordAfternoonEntry(DateTime.Today);
    }

    // todo: for testing, delete later
    public static void CalendarOpenedTest(DateTime day){
        CalendarOpenedOnDate(day);
    }
    public static void RecordAfternoonEntryTest(DateTime day){
        RecordAfternoonEntry(day);
    }
    //#endregion

    private static void CalendarOpenedOnDate(DateTime today){
         // compare today's date and last updated day (calendar)
        TimeSpan sinceLastPlayed = today.Subtract(DataManager.LastPlayedDate);

        if (sinceLastPlayed.Days == 0){ // if same day. no miss days
            SameDayGenerateEntry();
        }
        else if (sinceLastPlayed.Days >= 1){ //next day (missing 0 days) or missing >=1 days

            tempEntries = new List<CalendarEntry>(); //temp list for calculation only
            int missedDays = sinceLastPlayed.Days - 1; //don't consider today's entry until the very end

            if(missedDays <= 3){
                //if the player does not play for <=3 days, every dose of the drug
                //is taken by the pet with no misses

                //generate entries for the missed days
                //the oldest entries are generated first
                for(int i=missedDays; i>0; i--){
                    TimeSpan timeSpan = new TimeSpan(i, 0, 0, 0); //convert missed days to timespan
                    DateTime missedDate = today.Subtract(timeSpan);
                    tempEntries.Add(GenerateEntryWithNoPunishment(missedDate.DayOfWeek));
                }

            }else{
                //if player misses for >3 days the pet starts missing doses with 60%
                //frequency for each 12 h dose and incurring the health consequences of this.

                int counter; //use to tell how many missed day entries are without punishment
                //and how many are with punishment
                for(counter = missedDays; counter>=3; counter--){
                    TimeSpan timeSpan = new TimeSpan(counter, 0, 0, 0); //convert missed days to timespan
                    DateTime missedDate = today.Subtract(timeSpan);
                    tempEntries.Add(GenerateEntryWithNoPunishment(missedDate.DayOfWeek));
                }

                //entries that include the missing doses with 60% frequency
                for(int i=counter; i>0; i--){
                    TimeSpan timeSpan = new TimeSpan(counter, 0, 0, 0); //convert missed days to timespan
                    DateTime missedDate = today.Subtract(timeSpan);
                    tempEntries.Add(GenerateEntryWithPunishment(missedDate.DayOfWeek));
                }
            }

            //by now tempEntries should include all the entries for the missed days
            //generate entries for today. add to list and update LastPlayedDate
            GenerateEntry(today); // stored in todaysEntry
            tempEntries.Add(todaysEntry); //add todays entry back in tempEntries
            IsNewWeek(today); // add relevant entries from tempEntries to DataManager.Entries

            UpdateComboCountOnDate(today);
            DataManager.LastPlayedDate = today;
        }
    }

    private static void RecordAfternoonEntry(DateTime today){
        todaysEntry.Afternoon = DosageRecord.Hit;
        UpdateComboCountOnDate(today);
    }


    private static void UpdateComboCountOnDate(DateTime today){
        TimeSpan sinceLastCombo = today.Subtract(DataManager.LastComboDate);

        // update combo count
        if (sinceLastCombo.Days > 1){ // missed at least one day
            DataManager.ResetCalendarCombo();
        }
        // check if calendarCombo is already incremented for that day
        if (DataManager.LastComboDate != today){
            if (todaysEntry.Morning == DosageRecord.Hit && todaysEntry.Afternoon == DosageRecord.Hit){
                DataManager.IncrementCalendarCombo();
                DataManager.LastComboDate = today;
            }
        }
    }

    private static DayOfWeek GetDay(DateTime day){
        return day.DayOfWeek;
    }

    //Check if it is a new week. Figure out how many entries need to be displayed
    //in the new week
    private static void IsNewWeek(DateTime today){
        if(today > DataManager.DateOfSunday){
            //today's date is later than sunday

            TimeSpan sinceSunday = today - DataManager.DateOfSunday;

            //create new list for the new week
            //only move the latest entries into the new week
            DataManager.Entries = tempEntries.GetRange(tempEntries.Count - sinceSunday.Days, sinceSunday.Days);

            DataManager.DateOfSunday = GetDateOfSunday(today);

        }else{ //same week so all temporary entries get displayed
            DataManager.Entries.AddRange(tempEntries);
        }
    }

    private static CalendarEntry GenerateEntryWithNoPunishment(DayOfWeek day){
        return new CalendarEntry(day, DosageRecord.Hit, DosageRecord.Hit);
    }

    //60% frequency for each 12h dose: morning, afternoon
    private static CalendarEntry GenerateEntryWithPunishment(DayOfWeek day){
        DosageRecord morning, afternoon;
        morning = GetHitOrMiss(60);
        afternoon = GetHitOrMiss(60);

        return new CalendarEntry(day, morning, afternoon);
    }

    private static void SameDayGenerateEntry(){
        // check if morning or afternoon
        if (DateTime.Now.Hour < 12){ // morning
            // should be already generated, so do nothing
        }
        else { // afternoon
            todaysEntry.Afternoon = GetHitOrMiss(40);
        }
    }

    private static void GenerateEntry(DateTime today){
        DayOfWeek day = GetDay(today);

        DosageRecord morning, afternoon;
        if (DateTime.Now.Hour < 12){ // morning
            morning = GetHitOrMiss(40);
            afternoon = DosageRecord.Null;
        }
        else { // afternoon
            morning = DosageRecord.Hit;
            afternoon = GetHitOrMiss(40);
        }
        CalendarEntry newEntry = new CalendarEntry(day, morning, afternoon);
        todaysEntry = newEntry;

    }

    private static DosageRecord GetHitOrMiss(int missPercentage){
        int chance = rand.Next(100);
        if (chance < missPercentage){
            return DosageRecord.Miss;
        }
        return DosageRecord.Hit;

    }
}
