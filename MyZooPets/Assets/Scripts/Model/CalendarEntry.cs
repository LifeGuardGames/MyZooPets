using UnityEngine;
using System.Collections;
using System;

[DoNotSerializePublic]
public class CalendarEntry{
    [SerializeThis]
    private DayOfWeek day;
    [SerializeThis]
    private DosageRecord morning;
    [SerializeThis]
    private DosageRecord afternoon;
    [SerializeThis]
    private bool checkedInMorning; //if the player checked the game in the morning
    [SerializeThis]
    private bool checkedInAfternoon; //if the player checked the game in the afternoon

    public DayOfWeek Day{
        get{return day;}
    }

    public DosageRecord Morning{
        get { return this.morning;}
        set { this.morning = value;}
    }
    public DosageRecord Afternoon{
        get { return this.afternoon;}
        set { this.afternoon = value;}
    }
    public bool CheckedInMorning{
        get { return this.checkedInMorning;}
        set { this.checkedInMorning = value;}
    }
    public bool CheckedInAfternoon{
        get { return this.checkedInAfternoon;}
        set { this.checkedInAfternoon = value;}
    }

    public CalendarEntry(DayOfWeek day, DosageRecord morning, DosageRecord afternoon ){
        this.day = day;
        this.morning = morning;
        this.afternoon = afternoon;
        this.checkedInMorning = false;
        this.checkedInAfternoon = false;
    }
    //parameterless constructor required for Serializer
    public CalendarEntry(){}
}

