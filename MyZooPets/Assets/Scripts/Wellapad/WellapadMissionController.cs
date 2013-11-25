using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadMissionController
// Controller class for the wellapad's missions.
//---------------------------------------------------

// event args for OnTaskUpdated event
public class TaskUpdatedArgs : EventArgs {
	public string ID{get; set;}
	public string Mission{get; set;}
	public bool Status{get; set;}
}

// this is save date for a given mission
public class Mission{
    public string ID {get; set;}						// id of the mission
	public RewardStatuses RewardStatus {get; set;}		// status of the mission's reward
	public Dictionary<string, bool> Tasks {get; set;}	// all the tasks and their status for this mission

    public Mission(){}

    public Mission(string id, Dictionary<string, bool> tasks){
        ID = id;
		Tasks = tasks;
		RewardStatus = RewardStatuses.Unearned;
    }
}

public class WellapadMissionController : Singleton<WellapadMissionController> {
	//=======================Events========================
    public EventHandler<TaskUpdatedArgs> OnTaskUpdated;   // when a task's status is updated
	//=====================================================
	
	//---------------------------------------------------
	// GetMission()
	// Returns the user's save data for the incoming
	// missiong ID.
	//---------------------------------------------------	
	public Mission GetMission( string strID ) {
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strID ) )
			return DataManager.Instance.GameData.Wellapad.CurrentTasks[strID];
		else {
			Debug.Log("No such mission in current tasks: " + strID);
			return null;
		}
	}
	
	//---------------------------------------------------
	// UnlockTask()
	// Marks the incoming task as unlocked -- it may now
	// be eligible for being in a mission.
	//---------------------------------------------------	
	public void UnlockTask( string strTask ) {
		DataManager.Instance.GameData.Wellapad.TasksUnlocked.Add( strTask );
	}
	
	//---------------------------------------------------
	// ClaimReward()
	// Called when the user successfully claimed a reward
	// from a mission.
	//---------------------------------------------------	
	public void ClaimReward( string strMissionID ) {
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strMissionID ) ) {
			// one final check just to be safe
			if ( DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionID].RewardStatus == RewardStatuses.Unclaimed ) {
				// for now the only reward is breathing fire
				DataManager.Instance.GameData.PetInfo.ChangeFireBreaths( 1 );
				
				DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionID].RewardStatus = RewardStatuses.Claimed;
				
				Debug.Log("Reward claimed for mission: " + strMissionID);
			}
			else
				Debug.Log("Something trying to claim an unclaimable reward for mission: " + strMissionID);
		}
		else 
			Debug.Log("Something trying to claim a reward for non-current mission: " + strMissionID);
	}	
	
	//---------------------------------------------------
	// SetCurrentTasks()
	// Saves the list of incoming tasks to the user's
	// current tasks.  The status of all these tasks
	// is assumed to be false, since they are new.
	// So by definition, only call this function when you
	// are setting NEW tasks.  Otherwise, just manipulate
	// the save data.
	//---------------------------------------------------		
	public void SetCurrentTasks( string strType, List<Data_WellapadTask> listTasks ) {
		Dictionary<string, bool> savedTasks = new Dictionary<string, bool>();
		
		for ( int i = 0; i < listTasks.Count; ++i ) {
			Data_WellapadTask task = listTasks[i];
			string strID = task.GetID();
			
			savedTasks[strID] = false;
		}
		
		DataManager.Instance.GameData.Wellapad.CurrentTasks[strType] = new Mission( strType, savedTasks );
		
		// reset the time
		DataManager.Instance.GameData.Wellapad.DateMissionsCreated = DateTime.Now;
	}
	
	//---------------------------------------------------
	// TaskCompleted()
	// Called from various parts of the game when a task
	// is completed that may be an active mission for the
	// player.
	//---------------------------------------------------		
	public void TaskCompleted( string strCompleted ) {
		// check to see if the completed task was in a player's mission -- if it was, mark it was done
		foreach ( KeyValuePair<string, Mission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks ) {
			foreach ( KeyValuePair<string, bool> task in mission.Value.Tasks ) {
				if ( task.Key == strCompleted ) {
					// only do anything if that task was previously not completed
					if ( task.Value == false ) {
						DataManager.Instance.GameData.Wellapad.CurrentTasks[mission.Key].Tasks[task.Key] = true;
						
						// because the task was completed, we may have to update our reward status...
						// got to do this before sending the event or the reward will not be displayed right
						RewardCheck( mission.Key );						
						
						// send event for the task update
						if ( OnTaskUpdated != null ) {
							TaskUpdatedArgs args = new TaskUpdatedArgs();
							args.Status = true;
							args.ID = task.Key;
							args.Mission = mission.Key;
							OnTaskUpdated( this, args );
						}
					}
					
					// right now there will only be unique tasks for each mission, so if we've found it, just return out
					return;
				}
			}
		}
	}
	
	//---------------------------------------------------
	// RefreshCheck()
	// Function that checks to see if the wellapad missions
	// list should be refreshed.  If it should, it will
	// delete the current save data.
	//---------------------------------------------------		
	public void RefreshCheck() {
        DateTime now = DateTime.Now;
        TimeSpan sinceCreated = now - DataManager.Instance.GameData.Wellapad.DateMissionsCreated;
		
		// the list needs to be refreshed if it has been more than 12 hours from creation OR the creation time frame (morning/evening)
		// is different than the current time frame (morning/evening)
		bool bRefresh = sinceCreated.Hours >= 12 || CalendarLogic.GetTimeFrame( now ) != CalendarLogic.GetTimeFrame( DataManager.Instance.GameData.Wellapad.DateMissionsCreated );
		
		// if we have to refresh, just delete our data...the missions list will take it from there
		if ( bRefresh )
			DataManager.Instance.GameData.Wellapad.ResetMissions();
	}
	
	//---------------------------------------------------
	// RewardCheck()
	// This function was called because some task in
	// strMissionID was just completed, so we need to see
	// if that mission's reward needs a status update.  This
	// is kind of heavy handed, but I'm not sure how else
	// to do it at the moment.
	// Note that this doesn't actually effect the UI...it
	// just keeps our internal state correct.  It's done
	// this way because our save system needs ot save
	// primitives.
	//---------------------------------------------------		
	private void RewardCheck( string strMissionID ) {
		// do some legality checks

		// mission exists
		if ( !DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strMissionID ) ) {
			Debug.Log("Reward is attempted to be check for an illegal mission: " + strMissionID);
			return;
		}
		
		// reward is currently unearned
		Mission mission = DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionID];
		if ( mission.RewardStatus != RewardStatuses.Unearned ) {
			Debug.Log("Reward check revealed illegal state for reward for mission " + strMissionID);
			return;
		}
		
		// loop through all tasks to see their status...if we run into a task that is not completed, just return
		foreach ( KeyValuePair<string, bool> task in mission.Tasks ) {
			if ( task.Value == false )
				return;
		}
		
		// if we get here it means that all tasks in the mission are complete -- the reward is now unclaimed
		mission.RewardStatus = RewardStatuses.Unclaimed;
	}
	
	//---------------------------------------------------
	// GetTaskStatus()
	// Returns whether the user has completed the
	// incoming task or not.
	//---------------------------------------------------	
	public bool GetTaskStatus( Data_WellapadTask task ) {
		bool bStatus = false;
		
		string strType = task.GetTaskType();
		string strID = task.GetID();
		
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strType ) && 
				DataManager.Instance.GameData.Wellapad.CurrentTasks[strType].Tasks.ContainsKey( strID ) ) 
			bStatus = DataManager.Instance.GameData.Wellapad.CurrentTasks[strType].Tasks[strID];
		else
			Debug.Log("Can't find task " + task + " in saved data");
		
		return bStatus;
	}
	
	//---------------------------------------------------
	// GetTasks()
	// Returns a task list for a given mission type.
	// This will check to see if there is save data, and
	// use it if it exists, and create a fresh task list
	// if it does not.
	//---------------------------------------------------		
	public List<Data_WellapadTask> GetTasks( string strMissionType ) {
		List<Data_WellapadTask> listTasks = new List<Data_WellapadTask>();
			
		// before creating the missions from scratch, check to see if the user has any save data
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strMissionType ) ) {
			// user has saved tasks...use those	
			Dictionary<string, bool> savedTasks = DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionType].Tasks;
			
			// loop through all saved tasks and get the data for them
			foreach (KeyValuePair<string, bool> pair in savedTasks) {
				// get the task
			    Data_WellapadTask task = DataLoader_WellapadTasks.GetTask( strMissionType, pair.Key );
				
				// stuff it in the list
				if ( task != null )
					listTasks.Add( task );
			}
		}
		else {
			// otherwise, load them from scratch
			listTasks = DataLoader_WellapadTasks.GetTasks( strMissionType );
			
			// now that we've got the new mission list, we need to save it
			SetCurrentTasks( strMissionType, listTasks );
		}
		
		return listTasks;
	}	
}
