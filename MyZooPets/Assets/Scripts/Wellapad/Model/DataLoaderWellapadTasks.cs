using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Data loader wellapad tasks.
/// Hash -- Key: taskID, Value: ImmutableDataWellapadTask
/// </summary>
public class DataLoaderWellapadTasks : XMLLoaderGeneric<DataLoaderWellapadTasks>{

	//Hash -- Key: missionID, Value: (Hash -- Key: category , Value: List<ImmutableDataWellapadTask>)
	private static Hashtable missionTaskHash;

	/// <summary>
	/// Gets the task.
	/// </summary>
	/// <returns>The task.</returns>
	/// <param name="taskID">Task ID.</param>
	public static ImmutableDataWellapadTask GetTask(string taskID){
		instance.InitXMLLoader();

		return instance.GetData<ImmutableDataWellapadTask>(taskID);
	}

	/// <summary>
	/// Gets the tasks with missionID.
	/// </summary>
	/// <returns>The tasks.</returns>
	/// <param name="missionID">Mission ID.</param>
	public static Hashtable GetTasks(string missionID){
		instance.InitXMLLoader();
		instance.ForceSetup();
		
		Hashtable taskHash = null;
		if(missionTaskHash.ContainsKey(missionID)){
			// get the hashtable of this mission
			taskHash = (Hashtable) missionTaskHash[missionID];
		}
		else
			Debug.LogError("Error...no missions for " + missionID);		

		return taskHash;
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataWellapadTask data = new ImmutableDataWellapadTask(id, xmlNode, errorMessage);

		if(hashData.ContainsKey(id))
			Debug.LogError("Duplicate task id: " + id);
		else
			hashData[id] = data;

		SortData(data);
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Wellapad/Tasks";
	}

	/// <summary>
	/// Sorts the data. Easier for retrieval later
	/// Hash -- Key: missionID, Value: (Hash -- Key: category , Value: List<ImmutableDataWellapadTask>)
	/// </summary>
	/// <param name="data">Data.</param>
	private void SortData(ImmutableDataWellapadTask data){
		if(missionTaskHash == null)
			missionTaskHash = new Hashtable();

		string missionID = data.GetTaskType();
		if(!missionTaskHash.ContainsKey(missionID))
			missionTaskHash[missionID] = new Hashtable();
		
		// this is the hashtable of tasks in a mission
		Hashtable taskHash = (Hashtable) missionTaskHash[missionID];
		
		// now drill deeper -- we want to sort by categories
		string category = data.GetCategory();
		if(!taskHash.ContainsKey(category))
			taskHash[category] = new List<ImmutableDataWellapadTask>();
		
		List<ImmutableDataWellapadTask> tasks = (List<ImmutableDataWellapadTask>) taskHash[category];
		tasks.Add(data);
	}
}

