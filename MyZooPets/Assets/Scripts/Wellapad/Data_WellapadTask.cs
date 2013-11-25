using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Data_WellapadTask
// Individual piece of wellapad mission data form xml.
// Considered to be immutable.
//---------------------------------------------------

public class Data_WellapadTask {
	// id for the mission
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// the type of mission this is
	private string strTaskType;
	public string GetTaskType() {
		return strTaskType;
	}
	
	// mission text
	public string GetText() {
		string strKey = "Task_" + GetID();
		return Localization.Localize( strKey );	
	}
	
	// key to check for inclusion (optional)
	// if this is TRUE, then this task will not be added to a mission unless this key is present in the player's unlocked task data
	private string strInclusionKey;
	public string GetInclusionKey() {
		return strInclusionKey;	
	}
	
	// key to check for completion event
	public string GetCompletionKey() {
		string strKey = "TaskComplete_" + GetID();
		return strKey;	
	}	

	public Data_WellapadTask( string id, Hashtable hashAttr, Hashtable hashData, string strError ) {
		// set id
		strID = id;
		
		// get the mission type
		strTaskType = HashUtils.GetHashValue<string>( hashAttr, "Type", "Side", strError );
		
		// get the inclusion key (optional)
		bool bInclusion = XMLUtils.GetBool(hashData["InclusionCheck"] as IXMLNode, false);
		if ( bInclusion )
			strInclusionKey = id;
	}
}
