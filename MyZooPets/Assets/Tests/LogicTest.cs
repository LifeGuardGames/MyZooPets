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
        CalendarOpenedTestSkip5();
        PrintCalendarEntries();
        GetComboCountTest();
	}

	// Update is called once per frame
	void Update () {

	}

    // tested 6/3/2013 - works properly
    void GetDateOfSundayTest(){
        Debug.Log("Debugging GetDateOfSunday()");
        Debug.Log("GetDateOfSunday(DateTime.Today)... Day specified is a " + DateTime.Today.DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(1))... Day specified is a " + DateTime.Today.AddDays(1).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(1)));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(2))... Day specified is a " + DateTime.Today.AddDays(2).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(2)));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(3))... Day specified is a " + DateTime.Today.AddDays(3).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(3)));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(4))... Day specified is a " + DateTime.Today.AddDays(4).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(4)));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(5))... Day specified is a " + DateTime.Today.AddDays(5).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(5)));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(6))... Day specified is a " + DateTime.Today.AddDays(6).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(6)));
        Debug.Log("GetDateOfSunday(DateTime.Today.AddDays(7))... Day specified is a " + DateTime.Today.AddDays(7).DayOfWeek + ". Date of Sunday is " + CalendarLogic.GetDateOfSunday(DateTime.Today.AddDays(7)));
    }

    // tested 6/3/2013 - works properly
    void GetComboCountTest(){
        Debug.Log("Debugging GetComboCount()");
        Debug.Log("GetComboCount() -> " + CalendarLogic.GetComboCount());
    }

    // to populate calendar entries
    // tested 6/3/2013 - works properly
    void AddCalendarEntries(){
        List <CalendarEntry> entries = CalendarLogic.GetCalendarEntries();
        entries.Add(new CalendarEntry( DayOfWeek.Monday, DosageRecord.Hit, DosageRecord.Hit) );
        entries.Add(new CalendarEntry( DayOfWeek.Tuesday, DosageRecord.Hit, DosageRecord.Hit) );
        entries.Add(new CalendarEntry( DayOfWeek.Wednesday, DosageRecord.Hit, DosageRecord.Hit) );
        entries.Add(new CalendarEntry( DayOfWeek.Thursday, DosageRecord.Hit, DosageRecord.Hit) );
    }

    // tested 6/3/2013 - works properly
    void PrintCalendarEntries(){
        Debug.Log("Debugging GetCalendarEntries()");
        List <CalendarEntry> entries = CalendarLogic.GetCalendarEntries();
        for (int i = 0; i < entries.Count; i++){
            CalendarEntry entry = entries[i];
            Debug.Log(entry.Day + " -> " + entry.Morning + ", " + entry.Afternoon);
        }
    }

    // tested 6/3/2013 - works properly
    void RecordAfternoonEntryTest(DateTime day){
        CalendarLogic.RecordAfternoonEntryTest(day);
    }

    // tested 6/3/2013 - works properly
    void CalendarOpenedTestConsecutive(){
        CalendarLogic.CalendarOpenedTest(DateTime.Today);
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(1));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(2));
        RecordAfternoonEntryTest(DateTime.Today.AddDays(2));
        // CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(3));
        // CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(4));
        // RecordAfternoonEntryTest(DateTime.Today.AddDays(4));
    }

    // tested 6/3/2013 - works properly
    void CalendarOpenedTestSkip1(){
        CalendarLogic.CalendarOpenedTest(DateTime.Today);
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(2));
    }
    // tested 6/3/2013 - works properly
    void CalendarOpenedTestSkip2(){
        CalendarLogic.CalendarOpenedTest(DateTime.Today);
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(3));
    }
    // tested 6/3/2013 - works properly
    void CalendarOpenedTestSkip5(){
        CalendarLogic.CalendarOpenedTest(DateTime.Today);
        Debug.Log("Six days from today: " + DateTime.Today.AddDays(6).DayOfWeek);
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(6));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(7));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(8));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(9));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(10));
    }
}
