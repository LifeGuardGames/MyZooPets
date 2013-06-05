using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogicTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // AddCalendarEntries();
        // CalendarOpenedTestConsecutive();
        // AllHits();
        // Miss1Day();
        Miss1Period();
        // Miss2Days();
        // MissNDays(3);
        // MissMoreThan3Days();
        PrintCalendarEntries();
        GetComboCountTest();
        PrintScore();
	}

	// Update is called once per frame
	void Update () {

	}

    void GetDateOfSundayTest(){
        Debug.Log("Debugging GetDateOfSunday()");
        Debug.Log("GetDateOfSunday(DateTime.Now)... Day specified is a " + DateTime.Now.DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(1))... Day specified is a " + DateTime.Now.AddDays(1).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(1)));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(2))... Day specified is a " + DateTime.Now.AddDays(2).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(2)));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(3))... Day specified is a " + DateTime.Now.AddDays(3).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(3)));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(4))... Day specified is a " + DateTime.Now.AddDays(4).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(4)));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(5))... Day specified is a " + DateTime.Now.AddDays(5).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(5)));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(6))... Day specified is a " + DateTime.Now.AddDays(6).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(6)));
        Debug.Log("GetDateOfSunday(DateTime.Now.AddDays(7))... Day specified is a " + DateTime.Now.AddDays(7).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Now.AddDays(7)));
    }

    void GetComboCountTest(){
        Debug.Log("GetComboCount() -> " + CalendarLogic.GetComboCount());
    }
    void PrintScore(){
        Debug.Log("DataManager.Points() -> " + DataManager.Points);
    }

    // to populate calendar entries, without any combos or points
    // void AddCalendarEntries(){
    //     List <CalendarEntry> entries = CalendarLogic.GetCalendarEntries();
    //     entries.Add(new CalendarEntry( DayOfWeek.Monday, DosageRecord.Hit, DosageRecord.Hit) );
    //     entries.Add(new CalendarEntry( DayOfWeek.Tuesday, DosageRecord.Hit, DosageRecord.Hit) );
    //     entries.Add(new CalendarEntry( DayOfWeek.Wednesday, DosageRecord.Hit, DosageRecord.Hit) );
    //     entries.Add(new CalendarEntry( DayOfWeek.Thursday, DosageRecord.Hit, DosageRecord.Hit) );
    // }

    void PrintCalendarEntries(){
        Debug.Log("Debugging GetCalendarEntries()");
        List <CalendarEntry> entries = CalendarLogic.GetCalendarEntries();
        for (int i = 0; i < entries.Count; i++){
            CalendarEntry entry = entries[i];
            Debug.Log(entry.Day + " -> " + entry.Morning + " ("+ entry.OpenedInMorning +"), " + entry.Afternoon + " ("+ entry.OpenedInAfternoon +")");
        }
    }

    void RecordGivingInhalerIfNotTakenTest(DateTime now){
        CalendarEntry entry = CalendarLogic.LastEntryTest();
        if ((now.Hour < 12 && entry.Morning == DosageRecord.Miss) ||
            (now.Hour >= 12 && entry.Afternoon == DosageRecord.Miss)){
            CalendarLogic.RecordGivingInhalerTest(now);
        }
    }

    void CalendarOpenedTestConsecutive(){
        Debug.Log("Debugging CalendarOpenedTestConsecutive()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;

        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker); // 7PM

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        // CalendarLogic.CalendarOpenedTest(timeTracker);
    }

    void AllHits(){
        Debug.Log("Debugging AllHits()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;

        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker); // 7PM
        RecordGivingInhalerIfNotTakenTest(timeTracker);

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        RecordGivingInhalerIfNotTakenTest(timeTracker);

    }

    void Miss1Day(){
        Debug.Log("Debugging Miss1Day()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

    }
    void Miss2Days(){
        Debug.Log("Debugging Miss2Days()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

    }
    void Miss3Days(){
        Debug.Log("Debugging Miss3Days()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

    }
    void MissNDays(int numDays){
        Debug.Log("Debugging MissNDays("+ numDays +")");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        for (int i = 0; i < numDays; i++){
            timeTracker += twelveHourPeriod;
            timeTracker += twelveHourPeriod;
        }

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

    }
    void Miss1Period(){
        Debug.Log("Debugging Miss1Period()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        timeTracker += twelveHourPeriod;

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
    }
    void MissMoreThan3Days(){
        Debug.Log("Debugging MissMoreThan3Days()");
        DateTime timeTracker = DateTime.Today;
        TimeSpan twelveHourPeriod = new TimeSpan(12,0,0);

        timeTracker += new TimeSpan(7,0,0); // 7AM this morning;
        CalendarLogic.CalendarOpenedTest(timeTracker);
        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);

        // miss 6 days
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;
        timeTracker += twelveHourPeriod;

        timeTracker += twelveHourPeriod;
        CalendarLogic.CalendarOpenedTest(timeTracker);
    }
}
