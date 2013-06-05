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
    private bool calculatedInMorning;
    [SerializeThis]
    private bool calculatedInAfternoon;

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
    public bool CalculatedInMorning{
        get { return this.calculatedInMorning;}
        set { this.calculatedInMorning = value;}
    }
    public bool CalculatedInAfternoon{
        get { return this.calculatedInAfternoon;}
        set { this.calculatedInAfternoon = value;}
    }

    public CalendarEntry(DayOfWeek day, DosageRecord morning, DosageRecord afternoon ){
        this.day = day;
        this.morning = morning;
        this.afternoon = afternoon;
        this.calculatedInMorning = false;
        this.calculatedInAfternoon = false;
    }
    //parameterless constructor required for Serializer
    public CalendarEntry(){}
}

