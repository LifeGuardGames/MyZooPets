using UnityEngine;
using System.Collections;

/// <summary>
/// Mutable data wellapad task. Will persist through the game
/// </summary>

public class MutableDataWellapadTask{
	public string MissionID { get; set; }		// mission this task is a part of
	public string TaskID { get; set; }		// ID of this task
	public string TaskName { get; set; }		// name of task. Can be the same as ID
	public int Amount { get; set; }			// complete condition
	public WellapadTaskCompletionStates Completed { get; set; }		// has this task been completed?

	//---------------------------------------------------
	// WillComplete()
	// Returns if the incoming task and amount will complete
	// this task.
	//---------------------------------------------------		
	public bool WillComplete(string taskID, int amount){
		bool isCompleted = false;
		
		if(Completed == WellapadTaskCompletionStates.Uncompleted && TaskName == taskID && amount >= Amount)
			isCompleted = true;
		
		return isCompleted;
	}
	
	public MutableDataWellapadTask(){
	}
	
	public MutableDataWellapadTask(ImmutableDataWellapadTask data, 
	                               WellapadTaskCompletionStates completionStatus = WellapadTaskCompletionStates.Uncompleted){
		MissionID = data.GetTaskType();
		TaskID = data.GetTaskID();
		TaskName = data.GetTaskName();
		Amount = data.GetRandomCompleteCondition();
		Completed = completionStatus;
	}
}
