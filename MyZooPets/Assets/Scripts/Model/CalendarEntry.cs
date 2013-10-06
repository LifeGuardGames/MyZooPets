using UnityEngine;
using System.Collections;
using System;

public class CalendarEntry{
    public DosageRecord DayTime {get; set;} //dayTime dosage record: hit or miss
    public DosageRecord NightTime {get; set;} //nightTime dosage record: hit or miss
    public bool BonusCollectedDayTime {get; set;}
    public bool BonusCollectedNightTime {get; set;}

    //=====================================

    public CalendarEntry(){
        DayTime = DosageRecord.Null;
        NightTime = DosageRecord.Null;
        BonusCollectedDayTime = false;
        BonusCollectedNightTime = false;
    }
    public CalendarEntry(DosageRecord dayTime, DosageRecord nightTime ){
        DayTime = dayTime;
        NightTime = nightTime;
        BonusCollectedDayTime = false;
        BonusCollectedNightTime = false;
    }
}

