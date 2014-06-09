using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ImmutableDataWellapadTask
// Individual piece of wellapad mission data form xml.
// Considered to be immutable.
//---------------------------------------------------

public class ImmutableDataWellapadTask{
	
	private string taskID; // id for the task
	private string taskName; // the actual value that the task looks for when a task is completed
	private string taskCategory; // the category this task belongs to
	private string taskType; // the type of mission this is
	private List<int> completeConditions; // conditions to complete the task

	/// <summary>
	/// Gets the task ID.
	/// </summary>
	/// <returns>The task ID.</returns>
	public string GetTaskID(){
		return taskID;	
	}

	/// <summary>
	/// Gets the name of the task. Can be the same as the ID
	/// </summary>
	/// <returns>The task name.</returns>
	public string GetTaskName(){
		return taskName;	
	}

	/// <summary>
	/// Gets the category.
	/// </summary>
	/// <returns>The category.</returns>
	public string GetCategory(){
		return taskCategory;	
	}

	/// <summary>
	/// Gets the type of the task.
	/// </summary>
	/// <returns>The task type.</returns>
	public string GetTaskType(){
		return taskType;
	}
	
	// mission text
	public string GetText(){
		string localizationKey = "Task_" + GetTaskName();
		return Localization.Localize(localizationKey);	
	}

	/// <summary>
	/// Gets the random complete condition.
	/// </summary>
	/// <returns>The random complete condition.</returns>
	public int GetRandomCompleteCondition(){
		int amount = 0;
		
		if(completeConditions != null && completeConditions.Count > 0)
			amount = ListUtils.GetRandomElement<int>(completeConditions);
		
		return amount;	
	}
	
	// key to check for completion event
	public string GetCompletionKey(){
		string strKey = "TaskComplete_" + GetTaskName();
		return strKey;	
	}

	public ImmutableDataWellapadTask(string id, Hashtable hashData, string errorMessage){
		// set id
		taskID = id;
		
		// get the task completion name -- fallback to the id
		taskName = XMLUtils.GetString(hashData["Name"] as IXMLNode, "", errorMessage);
		
		// get the mission type
		taskType = XMLUtils.GetString(hashData["Type"] as IXMLNode, "Side", errorMessage);
		
		// get the category of this task
		taskCategory = XMLUtils.GetString(hashData["Category"] as IXMLNode, "", errorMessage);
		
		// get the amounts(optional)
		if(hashData.ContainsKey("CompleteConditions")){
			completeConditions = new List<int>();
			string strAmounts = XMLUtils.GetString(hashData["CompleteConditions"] as IXMLNode);
			string[] arrayAmounts = strAmounts.Split(","[0]);
			for(int i = 0; i < arrayAmounts.Length; ++i)
				completeConditions.Add(int.Parse(arrayAmounts[i]));
		}
	}
}
