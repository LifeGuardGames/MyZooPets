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

    public CalendarEntry(DayOfWeek day, DosageRecord morning, DosageRecord afternoon ){
        this.day = day;
        this.morning = morning;
        this.afternoon = afternoon;
    }

    public CalendarEntry(){
        
    }
}

