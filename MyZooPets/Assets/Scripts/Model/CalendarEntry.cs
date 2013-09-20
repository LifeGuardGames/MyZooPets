using UnityEngine;
using System.Collections;
using System;

[DoNotSerializePublic]
public class CalendarEntry{
    [SerializeThis]
    private DosageRecord dayTime; //dayTime dosage record: hit or miss
    [SerializeThis]
    private DosageRecord nightTime; //nightTime dosage record: hit or miss
    [SerializeThis]
    private bool bonusCollectedDayTime;
    [SerializeThis]
    private bool bonusCollectedNightTime;

    //===============Getters & Setters=============
    public DosageRecord DayTime{
        get { return this.dayTime;}
        set { this.dayTime = value;}
    }
    public DosageRecord NightTime{
        get { return this.nightTime;}
        set { this.nightTime = value;}
    }
    public bool BonusCollectedDayTime{
        get { return this.bonusCollectedDayTime;}
        set { this.bonusCollectedDayTime = value;}
    }
    public bool BonusCollectedNightTime{
        get { return this.bonusCollectedNightTime;}
        set { this.bonusCollectedNightTime = value;}
    }
    //=====================================

    public CalendarEntry(){
        this.dayTime = DosageRecord.Hit;
        this.nightTime = DosageRecord.Miss;
        this.bonusCollectedDayTime = false;
        this.bonusCollectedNightTime = false;
    }
    public CalendarEntry(DosageRecord dayTime, DosageRecord nightTime ){
        this.dayTime = dayTime;
        this.nightTime = nightTime;
        this.bonusCollectedDayTime = false;
        this.bonusCollectedNightTime = false;
    }
}

