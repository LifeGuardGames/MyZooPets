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
	private string position;

	public string TriggerID { get; set; }

	public Vector3 Position{
		get{
			return Constants.ParseVector3(position);	
		} 
	}

	public DegradData(string triggerID, string position){
		this.position = position;
		TriggerID = triggerID;
	}
}