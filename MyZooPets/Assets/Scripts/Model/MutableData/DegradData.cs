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
	public string TriggerID { get; set; }

	public int Partition { get; set; }

	private string position;
	public Vector3 Position{
		get{
			return Constants.ParseVector3(position);	
		} 
	}

	public DegradData(string triggerID, int partition, string position){
		this.position = position;
		Partition = partition;
		TriggerID = triggerID;
	}
}