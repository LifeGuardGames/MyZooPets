using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalendarLogic : MonoBehaviour {

    private static System.Random rand = new System.Random();
    private static CalendarEntry lastEntry;
    private static List<CalendarEntry> tempEntries;

    //#region API (use this for the UI)
    public static int GetComboCount(){
        return DataManager.CalendarCombo;
    }

    public static List<CalendarEntry> GetCalendarEntries(){
        return DataManager.Entries;
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

    // call after giving inhaler to pet
    // assume that we can only give an inhaler to the pet if it missed it
    public static void RecordGivingInhaler(){
        RecordGivingInhaler(DateTime.Now);
    }

    // call whenever opening calendar
    public static void CalendarOpened(){
        CalendarOpenedOnDate(DateTime.Now);
    }

    //#endregion

    //***********************************************************

    //#region API
    // todo: for testing, delete later
    public static void RecordGivingInhalerTest(DateTime now){
        RecordGivingInhaler(now);
    }
    // todo: for testing, delete later
    public static void CalendarOpenedTest(DateTime now){
        CalendarOpenedOnDate(now);
    }

    public static CalendarEntry LastEntryTest(){
        return lastEntry;
    }
    //#endregion
    //***********************************************************

    private static void RecordGivingInhaler(DateTime now){
        if (now.Hour < 12) {
            lastEntry.Morning = DosageRecord.Hit;
        }
        else if (now.Hour >= 12) {
            lastEntry.Afternoon = DosageRecord.Hit;
        }
        CalculateScoreForToday(now);
    }

    private static void CalendarOpenedOnDate(DateTime now){
        UpdateLastEntryReference();
         // compare today's date and last updated day (calendar)
        TimeSpan sinceLastPlayed = now.Date.Subtract(DataManager.LastCalendarOpenedTime.Date);

        if (sinceLastPlayed.Days == 0){ // if same day. no miss days
            SameDayGenerateEntry(now);
            CalculateScoreForToday(now);
        }
        else {
            tempEntries = new List<CalendarEntry>(); //temp list for calculation only
            if (sinceLastPlayed.Days == 1){ // next day
                // last played day
                GeneratePreviousAfternoon();
                CalculateForPreviousDay();
            }
            else if (sinceLastPlayed.Days > 1){ // missing >1 days
                // last played day
                GeneratePreviousAfternoon();
                CalculateForPreviousDay();

                // missed days except today
                GenerateMissedEntries(now);
                //by now tempEntries should include all the entries for the missed days
                CalculateForMissedEntries();

            }
            //generate entries for today. add to list and update LastCalendarOpenedTime
            GenerateEntry(now); // stored in lastEntry
            CalculateScoreForToday(now);
            tempEntries.Add(lastEntry); //add todays entry back in tempEntries
            IsNewWeek(now); // add relevant entries from tempEntries to DataManager.Entries

            DataManager.LastCalendarOpenedTime = now;
        }
    }

    private static void UpdateLastEntryReference(){
        List<CalendarEntry> list = DataManager.Entries;
        if (list != null && list.Count > 0){
            lastEntry = DataManager.Entries[DataManager.Entries.Count - 1];
        }
    }

    //********************************************
    // entry generation methods

    //Generate entry for the afternoon
    private static void SameDayGenerateEntry(DateTime now){
        // check if morning or afternoon
        if (now.Hour < 12){ // morning
            // should be already generated, so do nothing
        }
        else { // afternoon
            if (lastEntry.OpenedInAfternoon == false){
                lastEntry.OpenedInAfternoon = true;
                if (lastEntry.Morning == DosageRecord.Hit){
                    lastEntry.Afternoon = GetHitOrMiss(40);
                }
                else {
                    lastEntry.Afternoon = DosageRecord.Hit;
                }
            }
        }
    }

    // if player didn't play the previous afternoon, turn it into a hit
    private static void GeneratePreviousAfternoon(){
        if (lastEntry != null && lastEntry.Afternoon == DosageRecord.Null){
            if (lastEntry.Morning == DosageRecord.Null){
                // do nothing, as this means this entry is just a placeholder, and its DosageRecord.Null value should not be changed
            }
            else {
                lastEntry.Afternoon = DosageRecord.Hit;
            }
        }
    }

    private static void GenerateMissedEntries(DateTime now){
        TimeSpan sinceLastPlayed = now.Date.Subtract(DataManager.LastCalendarOpenedTime.Date);
        int missedDays = sinceLastPlayed.Days - 1; //don't consider today's entry until the very end
        int counter = 0; //use to tell how many missed day entries are without punishment
                        //and how many are with punishment

        //if the player does not play for <=3 days, every dose of the drug
        //is taken by the pet with no misses
        for(counter = 0; counter<3 && counter < missedDays; counter++){
            TimeSpan timeSpan = new TimeSpan(missedDays - counter, 0, 0, 0); //convert missed days to timespan
            DateTime missedDate = now.Subtract(timeSpan);
            //generate entries for the missed days
            //the oldest entries are generated first
            tempEntries.Add(GenerateEntryWithNoPunishment(missedDate.DayOfWeek));
        }

        //if player misses for >3 days the pet starts missing doses with 70%
        //frequency for each 12 h dose and incurring the health consequences of this.
        for(counter = counter; counter < missedDays; counter++){
            TimeSpan timeSpan = new TimeSpan(missedDays - counter, 0, 0, 0); //convert missed days to timespan
            DateTime missedDate = now.Subtract(timeSpan);
            CalendarEntry entry = GenerateEntryWithPunishment(missedDate.DayOfWeek);
            tempEntries.Add(entry);
        }
    }

    //generate entry with DosageRecord.Hit for morning and afternoon
    private static CalendarEntry GenerateEntryWithNoPunishment(DayOfWeek day){
        return new CalendarEntry(day, DosageRecord.Hit, DosageRecord.Hit);
    }

    //70% miss frequency for each 12h dose: morning, afternoon
    private static CalendarEntry GenerateEntryWithPunishment(DayOfWeek day){
        DosageRecord morning, afternoon;
        morning = GetHitOrMiss(70);
        afternoon = GetHitOrMiss(70);

        return new CalendarEntry(day, morning, afternoon);
    }

    private static void GenerateEntry(DateTime now){
        DayOfWeek day = GetDay(now);
        CalendarEntry newEntry = new CalendarEntry(day);

        DosageRecord morning, afternoon;
        if (now.Hour < 12){ // morning
            newEntry.OpenedInMorning = true;
            newEntry.Morning = GetHitOrMiss(40);
            newEntry.Afternoon = DosageRecord.Null;
        }
        else { // afternoon
            newEntry.Morning = DosageRecord.Hit;
            newEntry.OpenedInAfternoon = true;
            newEntry.Afternoon = GetHitOrMiss(40);
        }

        lastEntry = newEntry;
    }

    //********************************************
    // calculation methods

    private static void CalculateScoreForToday(DateTime now){
        // if morning, only morning dosage is generated
        if (now.Hour < 12){
            if (lastEntry.CalculatedInMorning == false && lastEntry.OpenedInMorning == true){
                if (lastEntry.Morning == DosageRecord.Hit){
                    DataManager.AddPoints(250);
                    DataManager.IncrementCalendarCombo();
                    lastEntry.CalculatedInMorning = true;
                }
            }
        }
        // note: if the user didn't check it in the morning, they lose the combo
        // if afternoon, both dosages are generated
        else if (now.Hour >= 12){
            if (lastEntry.CalculatedInMorning == false && lastEntry.OpenedInMorning == true){
                if (lastEntry.Morning == DosageRecord.Miss){
                    DataManager.SubtractHealth(20);
                    DataManager.SubtractMood(20);
                    DataManager.ResetCalendarCombo();
                    lastEntry.CalculatedInMorning = true;
                }
            }

            if (lastEntry.CalculatedInAfternoon == false && lastEntry.OpenedInAfternoon == true){
                if (lastEntry.Afternoon == DosageRecord.Hit){
                    DataManager.AddPoints(250);
                    DataManager.IncrementCalendarCombo();
                    lastEntry.CalculatedInAfternoon = true;
                }
            }
        }
    }

    // Only have to worry about penalties, as rewards should have been awarded already.
    // Check if last entry still has a miss in the afternoon. If so -20 health and -20 mood
    private static void CalculateForPreviousDay(){
        if (lastEntry == null) return;

        // if previous afternoon was skipped and previous morning was missed
        if (lastEntry.OpenedInAfternoon == false){
            DataManager.ResetCalendarCombo();
            if (lastEntry.OpenedInMorning && lastEntry.Morning == DosageRecord.Miss){
                DataManager.SubtractHealth(20);
                DataManager.SubtractMood(20);
                lastEntry.CalculatedInMorning = true;
            }
        }
        // previous afternoon was not skipped, but miss was not corrected
        else if (lastEntry.Afternoon == DosageRecord.Miss){
                DataManager.SubtractHealth(20);
                DataManager.SubtractMood(20);
                DataManager.ResetCalendarCombo();
                lastEntry.CalculatedInAfternoon = true;
        }

    }

    //reset combo or punish the player for missing days
    private static void CalculateForMissedEntries(){
        if (tempEntries.Count > 0){ // if >0 days missed
            DataManager.ResetCalendarCombo();
        }
        foreach (CalendarEntry entry in tempEntries){
            if (entry.Morning == DosageRecord.Miss){
                DataManager.SubtractHealth(20);
                DataManager.SubtractMood(20);
            }
            if (entry.Afternoon == DosageRecord.Miss){
                DataManager.SubtractHealth(20);
                DataManager.SubtractMood(20);
            }
        }
    }

    //********************************************
    // other methods

    private static DayOfWeek GetDay(DateTime day){
        return day.DayOfWeek;
    }

    private static DosageRecord GetHitOrMiss(int missPercentage){
        int chance = rand.Next(100);
        if (chance < missPercentage){
            return DosageRecord.Miss;
        }
        return DosageRecord.Hit;

    }

    //Check if it is a new week. Figure out how many entries need to be displayed
    //in the new week
    private static void IsNewWeek(DateTime now){
        if(now.Date > DataManager.DateOfSunday){
            //today's  date is later than sunday

            TimeSpan sinceSunday = now.Date - DataManager.DateOfSunday;

            //create new list for the new week
            //only move the latest entries into the new week
            DataManager.Entries = tempEntries.GetRange(tempEntries.Count - sinceSunday.Days, sinceSunday.Days);

            DataManager.DateOfSunday = GetDateOfSunday(now);

        }else{ //same week so all temporary entries get displayed
            DataManager.Entries.AddRange(tempEntries);
        }
    }

}
