using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CalenderLogic : MonoBehaviour {

    private System.Random rand = new System.Random();
    private CalendarEntry todaysEntry; //Not sure if this would work because the data would
                                        //need to be saved
    List<CalendarEntry> tempEntries;

    //#region API
    public int GetComboCount(){
        return DataManager.CalendarCombo;
    }

    public List<CalendarEntry> GetCalendarEntries(){
        return DataManager.Entries;
    }

    // if dateTime is a Sunday, return dateTime. Else, return the next Sunday.
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
    //#endregion

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    void FirstTimeToday(){
        // rewardGivenToday = false;
    }

    // void Calculate(){
    //     GenerateEntryToday();
    //     // if (!rewardGivenToday) {
    //         GiveReward();
    //     // }
    // }

    private DayOfWeek getDayToday(){
        return DateTime.Today.DayOfWeek;
    }

    private void CalendarOpened(){

        // compare today's date and last updated day (calendar)
        TimeSpan sinceLastPlayed = DateTime.Today.Subtract(DataManager.LastPlayedDate);

        if (sinceLastPlayed.Days == 0){ // if same day. no miss days
            SameDayGenerateEntry();
        }
        else if (sinceLastPlayed.Days >= 1){ //start missing days
            tempEntries = new List<CalendarEntry>(); //temp list for calculation only
            int missedDays = sinceLastPlayed.Days - 1; //don't consider today's entry until the very end

            if(missedDays <= 3){
                //if the player does not play for <=3 days, every dose of the drug
                //is taken by the pet with no misses

                //generate entries for the missed days
                //the oldest entries are generated first
                for(int i=missedDays; i>0; i--){
                    TimeSpan timeSpan = new TimeSpan(i, 0, 0, 0); //convert missed days to timespan
                    DateTime missedDate = DateTime.Today.Subtract(timeSpan);
                    tempEntries.Add(GenerateEntryWithNoPunishment(missedDate.DayOfWeek));
                }

            }else{
                //if player misses for >3 days the pet starts missing doses with 60%
                //frequency for each 12 h dose and incurring the health consequences of this.

                int counter; //use to tell how many missed day entries are without punishment
                //and how many are with punishment
                for(counter = missedDays; counter>=3; counter--){
                    TimeSpan timeSpan = new TimeSpan(counter, 0, 0, 0); //convert missed days to timespan
                    DateTime missedDate = DateTime.Today.Subtract(timeSpan);
                    tempEntries.Add(GenerateEntryWithNoPunishment(missedDate.DayOfWeek));
                }

                //entries that include the missing doses with 60% frequency
                for(int i=counter; i>0; i--){
                    TimeSpan timeSpan = new TimeSpan(counter, 0, 0, 0); //convert missed days to timespan
                    DateTime missedDate = DateTime.Today.Subtract(timeSpan);
                    tempEntries.Add(GenerateEntryWithPunishment(missedDate.DayOfWeek));
                }
            }
            //by now tempEntries should include all the entries for the missed days
            //generate entries for today. add to list and update LastPlayedDate
            GenerateEntryToday();
            tempEntries.Add(todaysEntry); //add todays entry back in tempEntries

            IsNewWeek();
            // DataManager.Entries.Add(todaysEntry);
            DataManager.LastPlayedDate = DateTime.Today;
        }
    }

    //Check if it is a new week. Figure out how many entries need to be displayed
    //in the new week
    //TO DO: when to set reset DateOfSunday
    private void IsNewWeek(){
        TimeSpan sinceLastPlayed = DateTime.Today.Subtract(DataManager.DateOfSunday);
        if(DateTime.Today > DataManager.DateOfSunday){
            //today's  date is later than sunday

            TimeSpan sinceSunday = DateTime.Today - DataManager.DateOfSunday;

            //create new list for the new week
            // todo: do we need this? Since we're assigning something to DataManager.Entries right afterwards
            DataManager.Entries = new List<CalendarEntry>();

            //only move the latest entries into the new week
            DataManager.Entries = tempEntries.GetRange(tempEntries.Count - sinceSunday.Days, sinceSunday.Days);

            DataManager.DateOfSunday = GetDateOfSunday(DateTime.Today);

            // sinceLastPlayed.Days
        }else{ //same week so all temporary entries get displayed
            DataManager.Entries.AddRange(tempEntries);
        }
    }

    // private DateTime FewDaysAgo(int days){
    //     TimeSpan timeSpan = new TimeSpan(days, 0, 0, 0); //convert missed days to timespan
    //     return DateTime.Today.Subtract(timeSpan);
    // }

    private CalendarEntry GenerateEntryWithNoPunishment(DayOfWeek day){
        return new CalendarEntry(day, DosageRecord.Hit, DosageRecord.Hit);
    }

    //60% frequency for each 12h dose: morning, afternoon
    private CalendarEntry GenerateEntryWithPunishment(DayOfWeek day){
        DosageRecord morning, afternoon;
        morning = GetHitOrMiss(60);
        afternoon = GetHitOrMiss(60);

        return new CalendarEntry(day, morning, afternoon);
    }

    private void SameDayGenerateEntry(){
        // check if morning or afternoon
        if (DateTime.Now.Hour < 12){ // morning
            // should be already generated, so do nothing
        }
        else { // afternoon
            todaysEntry.Afternoon = GetHitOrMiss(40);
        }
    }

    private void GenerateEntryToday(){
        DayOfWeek day = getDayToday();

        DosageRecord morning, afternoon;
        if (DateTime.Now.Hour < 12){ // morning
            morning = GetHitOrMiss(40);
            afternoon = DosageRecord.Null;
        }
        else { // afternoon
            morning = DosageRecord.Hit;
            afternoon = GetHitOrMiss(40);
        }
        CalendarEntry today = new CalendarEntry(day, morning, afternoon);
        todaysEntry = today;

    }

    private DosageRecord GetHitOrMiss(int missPercentage){
        int chance = rand.Next(100);
        if (chance < missPercentage){
            return DosageRecord.Miss;
        }
        return DosageRecord.Hit;

    }

    private void GiveReward(){
        // rewardGivenToday = true;
    }



}
