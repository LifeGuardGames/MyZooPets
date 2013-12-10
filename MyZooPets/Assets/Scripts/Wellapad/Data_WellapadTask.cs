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
	
	// the category this task belongs to
	private string strCategory;
	public string GetCategory() {
		return strCategory;	
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
	
	// optional amount parameter this task may have
	private List<int> listAmounts;
	public int GetRandomAmount() {
		int nAmount = 0;
		
		if ( listAmounts.Count > 0 )
			nAmount = ListUtils.GetRandomElement<int>( listAmounts );
		
		return nAmount;	
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
		
		// get the category of this task
		strCategory = HashUtils.GetHashValue<string>( hashAttr, "Category", "" );
		
		// get the amounts(optional)
		listAmounts = new List<int>();
		string strAmounts = XMLUtils.GetString(hashData["Amounts"] as IXMLNode, "10");
		string[] arrayAmounts = strAmounts.Split( ","[0] );
		for ( int i = 0; i < arrayAmounts.Length; ++i )
			listAmounts.Add( int.Parse( arrayAmounts[i] ) );
	}
}
