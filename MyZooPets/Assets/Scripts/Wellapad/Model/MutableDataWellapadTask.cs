using UnityEngine;
using System.Collections;

/// <summary>
/// Mutable data wellapad task. Will persist through the game
/// </summary>

public class MutableDataWellapadTask{
	public string TaskID { get; set; }		// ID of this task
	public MiniGameCategory Category { get; set; }		// name of task. Can be the same as ID
	public int Amount { get; set; }			// complete condition
	public WellapadTaskCompletionStates Completed { get; set; }     // has this task been completed?
	public RewardStatuses isReward;

	//---------------------------------------------------
	// WillComplete()
	// Returns if the incoming task and amount will complete
	// this task.
	//---------------------------------------------------		
	public bool WillComplete(string taskID, int amount) {
		bool isCompleted = false;

		if(Completed == WellapadTaskCompletionStates.Uncompleted && TaskID == taskID && amount >= Amount) { 
			isCompleted = true;
			isReward = RewardStatuses.Unclaimed;
		}
		
		return isCompleted;
	}
	
	public MutableDataWellapadTask(){
	}
	
	public MutableDataWellapadTask(ImmutableDataWellapadTask data, WellapadTaskCompletionStates completionStatus = WellapadTaskCompletionStates.Uncompleted){
		TaskID = data.GetTaskID();
		if(data.GetCategory() == "Critical") {
			Category = MiniGameCategory.Critical;
		}
		else {
			Category = MiniGameCategory.Regular;
		}
		Amount = data.GetRandomCompleteCondition();
		Completed = completionStatus;
	}
}
