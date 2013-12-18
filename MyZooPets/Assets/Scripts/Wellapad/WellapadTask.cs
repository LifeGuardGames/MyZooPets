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
	public string MissionID {get; set;}		// mission this task is a part of
	public string TaskID {get; set;}		// ID of this task
	public string TaskName {get; set;}		// "name id" of the task for the completion message (not unique among tasks)
	public int Amount {get; set;}			// amount for this task
	public WellapadTaskCompletionStates Completed {get; set;}		// has this task been completed?

	//private Data_WellapadTask data;		// the raw, immutable data for this task 
	
	//---------------------------------------------------
	// WillComplete()
	// Returns if the incoming task and amount will complete
	// this task.
	//---------------------------------------------------		
	public bool WillComplete( string strID, int nAmount ) {
		bool bWillComplete = false;
		
		if ( Completed == WellapadTaskCompletionStates.Uncompleted && TaskName == strID && nAmount >= Amount )
			bWillComplete = true;
		
		return bWillComplete;
	}
	
	//---------------------------------------------------
	// GetDesc()
	// Returns the string description for this task.
	//---------------------------------------------------		
	public string GetDesc() {
		Data_WellapadTask data = DataLoader_WellapadTasks.GetTask( TaskID );
		string strDesc = data.GetText();
		
		// if the task has an amount, we want to integrate that into the string
		if ( Amount > 0 )
			strDesc = StringUtils.Replace( strDesc, StringUtils.NUM, Amount );
		
		return strDesc;
	}
	
	public WellapadTask() {}
	
	public WellapadTask( Data_WellapadTask data, WellapadTaskCompletionStates eCompleted = WellapadTaskCompletionStates.Uncompleted ) {
		MissionID = data.GetTaskType();
		TaskID = data.GetTaskID();
		TaskName = data.GetTaskName();
		Amount = data.GetRandomAmount();
		Completed = eCompleted;
	}
}
