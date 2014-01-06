using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CalendarData 
// Save the data for Calendar. Mutable data
//---------------------------------------------------

public class CalendarData{
    public DateTime NextPlayPeriod {get; set;} //the next time that the user can collect check bonuses

    //================Initialization============
    public CalendarData(){}

    public void Init(){
        NextPlayPeriod = PlayPeriodLogic.GetCurrentPlayPeriod();
    }
}
