using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogicTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // AddCalendarEntries();
        CalendarOpenedTestConsecutive();
        // CalendarOpenedTestSkip1();
        // CalendarOpenedTestSkip2();
        // CalendarOpenedTestSkip5();
        PrintCalendarEntries();
        GetComboCountTest();
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

    // to populate calendar entries, without any combos or points
    void AddCalendarEntries(){
        List <CalendarEntry> entries = CalendarLogic.GetCalendarEntries();
        entries.Add(new CalendarEntry( DayOfWeek.Monday, DosageRecord.Hit, DosageRecord.Hit) );
        entries.Add(new CalendarEntry( DayOfWeek.Tuesday, DosageRecord.Hit, DosageRecord.Hit) );
        entries.Add(new CalendarEntry( DayOfWeek.Wednesday, DosageRecord.Hit, DosageRecord.Hit) );
        entries.Add(new CalendarEntry( DayOfWeek.Thursday, DosageRecord.Hit, DosageRecord.Hit) );
    }

    void PrintCalendarEntries(){
        Debug.Log("Debugging GetCalendarEntries()");
        List <CalendarEntry> entries = CalendarLogic.GetCalendarEntries();
        for (int i = 0; i < entries.Count; i++){
            CalendarEntry entry = entries[i];
            Debug.Log(entry.Day + " -> " + entry.Morning + ", " + entry.Afternoon);
        }
    }

    void RecordGivingInhalerTest(DateTime now){
        CalendarLogic.RecordGivingInhalerTest(now);
    }

    void CalendarOpenedTestConsecutive(){
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

    void CalendarOpenedTestSkip1(){
        CalendarLogic.CalendarOpenedTest(DateTime.Now);
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(2));
    }
    void CalendarOpenedTestSkip2(){
        CalendarLogic.CalendarOpenedTest(DateTime.Now);
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(3));
    }
    void CalendarOpenedTestSkip5(){
        CalendarLogic.CalendarOpenedTest(DateTime.Now);
        Debug.Log("Six days from today: " + DateTime.Now.AddDays(6).DayOfWeek);
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(6));
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(7));
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(8));
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(9));
        CalendarLogic.CalendarOpenedTest(DateTime.Now.AddDays(10));
    }
}
