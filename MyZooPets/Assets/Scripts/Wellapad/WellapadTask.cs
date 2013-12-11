using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadTask
// This is save data for a wellapad task -- it will
// persist through the game and is NOT immutable.
// That would be the Data_WellapadTask.  This class
// holds the user's progress for the task.
//---------------------------------------------------

public class WellapadTask {
	public string MissionID {get {return data.GetTaskType();}}		// mission this task is a part of
	public string TaskID {get { return data.GetID();}}		// ID of this task
	public int Amount {get; set;}			// amount for this task
	public WellapadTaskCompletionStates Completed {get; set;}		// has this task been completed?

	private Data_WellapadTask data;		// the raw, immutable data for this task 
	
	//---------------------------------------------------
	// WillComplete()
	// Returns if the incoming task and amount will complete
	// this task.
	//---------------------------------------------------		
	public bool WillComplete( string strID, int nAmount ) {
		bool bWillComplete = false;
		
		if ( Completed == WellapadTaskCompletionStates.Uncompleted && TaskID == strID && nAmount >= Amount )
			bWillComplete = true;
		
		return bWillComplete;
	}
	
	//---------------------------------------------------
	// GetDesc()
	// Returns the string description for this task.
	//---------------------------------------------------		
	public string GetDesc() {
		string strDesc = data.GetText();
		
		// if the task has an amount, we want to integrate that into the string
		if ( Amount > 0 )
			strDesc = StringUtils.Replace( strDesc, StringUtils.NUM, Amount );
		
		return strDesc;
	}
	
	public WellapadTask() {}
	
	public WellapadTask( Data_WellapadTask data, WellapadTaskCompletionStates eCompleted = WellapadTaskCompletionStates.Uncompleted ) {
		this.data = data;
		Amount = data.GetRandomAmount();
		Completed = eCompleted;
	}
}
