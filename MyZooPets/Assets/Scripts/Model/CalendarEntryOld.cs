using UnityEngine;
using System.Collections;
using System;

[DoNotSerializePublic]
public class CalendarEntryOld{
    [SerializeThis]
    private DayOfWeek day; //day of the entry
    [SerializeThis]
    private DosageRecord morning; //morning dosage record: hit or miss
    [SerializeThis]
    private DosageRecord afternoon; //afternoon dosage record: hit or miss
    [SerializeThis]
    private bool openedInMorning; //has the entry been seen by the user in morning
    [SerializeThis]
    private bool openedInAfternoon; //has the entry been seen by user in the afternoon
    [SerializeThis]
    private bool calculatedInMorning; //has the stats/score been calculated in morning
    [SerializeThis]
    private bool calculatedInAfternoon; //has stats/score been calculated in afternoon

    //===============Getters & Setters=============
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
    public bool OpenedInMorning{
        get { return this.openedInMorning;}
        set { this.openedInMorning = value;}
    }
    public bool OpenedInAfternoon{
        get { return this.openedInAfternoon;}
        set { this.openedInAfternoon = value;}
    }
    public bool CalculatedInMorning{
        get { return this.calculatedInMorning;}
        set { this.calculatedInMorning = value;}
    }
    public bool CalculatedInAfternoon{
        get { return this.calculatedInAfternoon;}
        set { this.calculatedInAfternoon = value;}
    }
    //=====================================

    public CalendarEntryOld(DayOfWeek day){
        this.day = day;
        this.morning = DosageRecord.Null;
        this.afternoon = DosageRecord.Null;
        this.openedInMorning = false;
        this.openedInAfternoon = false;
        this.calculatedInMorning = false;
        this.calculatedInAfternoon = false;
    }
    public CalendarEntryOld(DayOfWeek day, DosageRecord morning, DosageRecord afternoon ){
        this.day = day;
        this.morning = morning;
        this.afternoon = afternoon;
        this.openedInMorning = false;
        this.openedInAfternoon = false;
        this.calculatedInMorning = false;
        this.calculatedInAfternoon = false;
    }
    //parameterless constructor required for Serializer
    public CalendarEntryOld(){}
}
