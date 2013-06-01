using UnityEngine;
using System.Collections;
using System;

public class CalendarEntry{
    private DayOfWeek day;
    private DosageRecord morning;
    private DosageRecord afternoon;

    public DayOfWeek Day{
        get{return day;}
    }

    public DosageRecord Morning{get; set;}
    public DosageRecord Afternoon{get; set;}

    public CalendarEntry(DayOfWeek day, DosageRecord morning, DosageRecord afternoon ){
        this.day = day;
        this.morning = morning;
        this.afternoon = afternoon;
    }
}

