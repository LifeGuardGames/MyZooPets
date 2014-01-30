using UnityEngine;
using System.Collections;
using System;

//----------------------------------------------------
// DegradData
// Only use for temp storage while the game is running.
// Not serialize
// Mutable data
//----------------------------------------------------
public class DegradData{
    public string TriggerID {get; set;}
	
	private string strPosition;
	public Vector3 GetPosition() {
		return Constants.ParseVector3( strPosition );	
	}

    public DegradData(string triggerID, string position){
        strPosition = position;
        TriggerID = triggerID;
    }
}