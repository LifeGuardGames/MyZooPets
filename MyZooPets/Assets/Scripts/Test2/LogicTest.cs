using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogicTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // AddCalendarEntries();
        CalendarOpenedTest();
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

    void GetComboCountTest(){
        Debug.Log("Debugging GetComboCount()");
        Debug.Log("GetComboCount() -> " + CalendarLogic.GetComboCount());
    }

    // to populate calendar entries
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

    // tested 6/3/2013 - works properly
    void CalendarOpenedTest(){
        CalendarLogic.CalendarOpenedTest(DateTime.Today);
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(1));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(2));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(3));
        CalendarLogic.CalendarOpenedTest(DateTime.Today.AddDays(4));
    }
}
