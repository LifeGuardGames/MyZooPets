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
    // assume that we can only give an inhaler to the pet if it missed it
    public static void RecordGivingInhaler(){
        if (DateTime.Now.Hour < 12) {
            lastEntry.Morning = DosageRecord.Hit;
        }
        else if (DateTime.Now.Hour >= 12) {
            lastEntry.Afternoon = DosageRecord.Hit;
        }
        DataManager.IncrementCalendarCombo();
    }

    // todo: for testing, delete later
    public static void CalendarOpenedTest(DateTime day){
        CalendarOpenedOnDate(day);
    }
    //#endregion
    //***********************************************************

    private static void UpdateLastEntryReference(){
        List<CalendarEntry> list = DataManager.Entries;
        if (list != null && list.Count > 0){
            lastEntry = DataManager.Entries[DataManager.Entries.Count - 1];
        }
    }

    private static void CalendarOpenedOnDate(DateTime today){
        UpdateLastEntryReference();
         // compare today's date and last updated day (calendar)
        TimeSpan sinceLastPlayed = today.Subtract(DataManager.LastPlayedDate);

        if (sinceLastPlayed.Days == 0){ // if same day. no miss days
            SameDayGenerateEntry();
            CalculateScoreForToday(today);
        }
        else {
            tempEntries = new List<CalendarEntry>(); //temp list for calculation only
            if (sinceLastPlayed.Days == 1){ // next day
                // last played day
                GeneratePreviousAfternoon();
                CalculateForPreviousAfternoon();
            }
            else if (sinceLastPlayed.Days > 1){ // missing >1 days
                // last played day
                GeneratePreviousAfternoon();
                CalculateForPreviousAfternoon();

                // missed days except today
                GenerateMissedEntries(today);
                //by now tempEntries should include all the entries for the missed days
                CalculateForMissedEntries();

            }
            //generate entries for today. add to list and update LastPlayedDate
            // todo: change back to orignal method
            GenerateEntryNow(today); // stored in lastEntry
            CalculateScoreForToday(today);
            tempEntries.Add(lastEntry); //add todays entry back in tempEntries
            IsNewWeek(today); // add relevant entries from tempEntries to DataManager.Entries

            // // todo: change
            // UpdateComboCountAtTime(today);
            DataManager.LastPlayedDate = today;
        }
    }

    private static void CalculateScoreForToday(DateTime now){
        int points = 0;
        // if morning, only morning dosage is generated
        if (now.Hour < 12 && lastEntry.Morning == DosageRecord.Hit){
            points += 250;
            DataManager.IncrementCalendarCombo();
            DataManager.LastComboTime = now;
        }
        // note: if the user didn't check it in the morning, they lose the combo
        // if afternoon, both dosages are generated
        else if (now.Hour >= 12 && lastEntry.Afternoon == DosageRecord.Hit){
            if (lastEntry.Morning == DosageRecord.Miss){
                DataManager.ResetCalendarCombo();
            }
            points += 250;
            DataManager.IncrementCalendarCombo();
            DataManager.LastComboTime = now;
        }
        DataManager.AddPoints(points);
    }

    private static void GenerateMissedEntries(DateTime today){
        TimeSpan sinceLastPlayed = today.Subtract(DataManager.LastPlayedDate);
        int missedDays = sinceLastPlayed.Days - 1; //don't consider today's entry until the very end
        int counter = 0; //use to tell how many missed day entries are without punishment
                        //and how many are with punishment

        //if the player does not play for <=3 days, every dose of the drug
        //is taken by the pet with no misses
        for(counter = 0; counter<3 && counter < missedDays; counter++){
            TimeSpan timeSpan = new TimeSpan(missedDays - counter, 0, 0, 0); //convert missed days to timespan
            DateTime missedDate = today.Subtract(timeSpan);
            //generate entries for the missed days
            //the oldest entries are generated first
            tempEntries.Add(GenerateEntryWithNoPunishment(missedDate.DayOfWeek));
        }

        //if player misses for >3 days the pet starts missing doses with 60%
        //frequency for each 12 h dose and incurring the health consequences of this.
        for(counter = counter; counter < missedDays; counter++){
            TimeSpan timeSpan = new TimeSpan(missedDays - counter, 0, 0, 0); //convert missed days to timespan
            DateTime missedDate = today.Subtract(timeSpan);
            CalendarEntry entry = GenerateEntryWithPunishment(missedDate.DayOfWeek);
            tempEntries.Add(entry);
        }
    }

    //Check if last entry still has a miss in the afternoon. If so -20 health and -20 mood
    private static void CalculateForPreviousAfternoon(){
        if (lastEntry == null) return;
        if (lastEntry.Morning == DosageRecord.Miss || lastEntry.Afternoon == DosageRecord.Miss){
            DataManager.SubtractHealth(20);
            DataManager.SubtractMood(20);
            DataManager.ResetCalendarCombo();
        }
    }

    //reset combo or punish the player for missing days
    private static void CalculateForMissedEntries(){
        if (tempEntries.Count > 0){ // if >0 days missed
            DataManager.ResetCalendarCombo();
        }
        foreach (CalendarEntry entry in tempEntries){
            if (entry.Morning == DosageRecord.Miss || entry.Afternoon == DosageRecord.Miss){
                DataManager.SubtractHealth(20);
                DataManager.SubtractMood(20);
            }
        }
    }


    // // todo: change
    // private static void UpdateComboCountAtTime(DateTime now){
    //     // if LastComboTime is in last 12 hour period,
    //     TimeSpan sinceLastCombo = now.Subtract(DataManager.LastComboTime);

    //     // update combo count
    //     if (sinceLastCombo.Days > 1){ // missed at least one day
    //         DataManager.ResetCalendarCombo();
    //     }
    //     if (DataManager.LastPlayedDate == now.AddDays(-1)){

    //         // todo
    //     }
    //     // check if calendarCombo is already incremented for that day
    //     if (DataManager.LastComboTime != now){
    //         if (lastEntry.Morning == DosageRecord.Hit && lastEntry.Afternoon == DosageRecord.Hit){
    //             // todo
    //             if (DataManager.LastPlayedDate == now){
    //                 DataManager.AddMood(10); // +10 mood for administering a missed dose
    //             }
    //             DataManager.AddMood(5); // +5 mood for checking diary every day
    //             DataManager.IncrementCalendarCombo();
    //             DataManager.LastComboTime = DateTime.Now;
    //         }
    //     }
    // }

    private static DayOfWeek GetDay(DateTime day){
        return day.DayOfWeek;
    }

    //Check if it is a new week. Figure out how many entries need to be displayed
    //in the new week
    private static void IsNewWeek(DateTime today){
        if(today > DataManager.DateOfSunday){
            //today's  date is later than sunday

            TimeSpan sinceSunday = today - DataManager.DateOfSunday;

            //create new list for the new week
            //only move the latest entries into the new week
            DataManager.Entries = tempEntries.GetRange(tempEntries.Count - sinceSunday.Days, sinceSunday.Days);

            DataManager.DateOfSunday = GetDateOfSunday(today);

        }else{ //same week so all temporary entries get displayed
            DataManager.Entries.AddRange(tempEntries);
        }
    }

    //generate entry with DosageRecord.Hit for morning and afternoon
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

    // if player didn't play the previous afternoon, turn it into a hit
    private static void GeneratePreviousAfternoon(){
        if (lastEntry != null && lastEntry.Afternoon == DosageRecord.Null){
            lastEntry.Afternoon = DosageRecord.Hit;
            DataManager.ResetCalendarCombo();
        }
    }

    //Generate entry for the afternoon
    private static void SameDayGenerateEntry(){
        // check if morning or afternoon
        if (DateTime.Now.Hour < 12){ // morning
            // should be already generated, so do nothing
        }
        else { // afternoon
            if (lastEntry.Morning == DosageRecord.Hit){
                lastEntry.Afternoon = GetHitOrMiss(40);
            }
            else {
                lastEntry.Afternoon = DosageRecord.Hit;
            }
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
            DataManager.ResetCalendarCombo();
            morning = DosageRecord.Hit;
            afternoon = GetHitOrMiss(40);
        }
        CalendarEntry newEntry = new CalendarEntry(day, morning, afternoon);
        lastEntry = newEntry;

    }
    //todo: testing; delete when done
    private static void GenerateEntryNow(DateTime now){
        DayOfWeek day = GetDay(now);

        DosageRecord morning, afternoon;
        if (now.Hour < 12){ // morning
            morning = GetHitOrMiss(40);
            afternoon = DosageRecord.Null;
        }
        else { // afternoon
            DataManager.ResetCalendarCombo();
            morning = DosageRecord.Hit;
            afternoon = GetHitOrMiss(40);
        }
        CalendarEntry newEntry = new CalendarEntry(day, morning, afternoon);
        lastEntry = newEntry;

    }

    private static DosageRecord GetHitOrMiss(int missPercentage){
        int chance = rand.Next(100);
        if (chance < missPercentage){
            return DosageRecord.Miss;
        }
        return DosageRecord.Hit;

    }


}
