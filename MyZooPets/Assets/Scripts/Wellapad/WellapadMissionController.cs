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

public class WellapadMissionController : Singleton<WellapadMissionController> {
	//=======================Events========================
    public EventHandler<TaskUpdatedArgs> OnTaskUpdated;   	// when a task's status is updated
	public EventHandler<EventArgs> OnMissionsRefreshed;		// when missions get refreshed
	public EventHandler<TaskUpdatedArgs> OnHighlightTask;	// when a certain task needs to be highlighted
	public EventHandler<EventArgs> OnRewardClaimed;			// when a reward is claimed
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
			Debug.LogError("No such mission in current tasks: " + strID);
			return null;
		}
	}
	
	//---------------------------------------------------
	// HasActiveTasks()
	// This function will return true if at least one
	// mission the user has is either unclaimed or
	// unearned reward.
	//---------------------------------------------------		
	public bool HasActiveTasks() {
		// start off assuming inactive
		bool bActive = false;
		
		// loop through all missions and check their reward status
		foreach ( KeyValuePair<string, Mission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks ) {
			Mission thisMission = mission.Value;
			if ( thisMission.RewardStatus == RewardStatuses.Unearned || thisMission.RewardStatus == RewardStatuses.Unclaimed )
				bActive = true;
		}

		return bActive;
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
				//StatsController.Instance.ChangeFireBreaths( 1 );
				
				int nXP = DataLoader_XpRewards.GetXP( "WellapadBonus", new Hashtable() );
				
				// get the position of the actual reward object because we want to stream the XP from it
				GameObject goReward = GameObject.Find("WellapadRewardButton");				
				Vector3 vPos = LgNGUITools.GetScreenPosition( goReward );
				vPos = CameraManager.Instance.TransformAnchorPosition( vPos, InterfaceAnchors.Center, InterfaceAnchors.Top );

				StatsController.Instance.ChangeStats( nXP, vPos, 0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero );
				DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionID].RewardStatus = RewardStatuses.Claimed;

				//Send analytics event
				Analytics.Instance.ClaimWellapadBonusXP();
				
				if ( OnRewardClaimed != null )
					OnRewardClaimed( this, EventArgs.Empty );
				
				//Debug.Log("Reward claimed for mission: " + strMissionID);
			}
			else
				Debug.LogError("Something trying to claim an unclaimable reward for mission: " + strMissionID);
		}
		else 
			Debug.LogError("Something trying to claim a reward for non-current mission: " + strMissionID);
	}	
	
	//---------------------------------------------------
	// TaskCompleted()
	// Called from various parts of the game when a task
	// is completed that may be an active mission for the
	// player.
	//---------------------------------------------------		
	public void TaskCompleted( string strCompleted, int nAmount = 0 ) {
		// check to see if the completed task was in a player's mission -- if it was, mark it was done
		foreach ( KeyValuePair<string, Mission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks ) {
			foreach ( KeyValuePair<string, WellapadTask> task in mission.Value.Tasks ) {
				if ( task.Value.WillComplete( strCompleted, nAmount ) ) {
					string strID = task.Value.TaskID;
					DataManager.Instance.GameData.Wellapad.CurrentTasks[mission.Key].Tasks[strID].Completed = WellapadTaskCompletionStates.RecentlyCompleted;

					// because the task was completed, we may have to update our reward status...
					// got to do this before sending the event or the reward will not be displayed right
					RewardCheck( mission.Key );						

					//Analytics
					Analytics.Instance.WellapadTaskEvent(Analytics.TASK_STATUS_COMPLETE, task.Value.MissionID, task.Value.TaskID);
					
					// send event for the task update
					if ( OnTaskUpdated != null ) {
						TaskUpdatedArgs args = new TaskUpdatedArgs();
						args.Status = true;
						args.ID = strCompleted;
						args.Mission = mission.Key;
						OnTaskUpdated( this, args );
					}
					
					// right now there will only be unique tasks for each mission, so if we've found it, just return out
					return;
				}
			}
		}
	}

	void Awake(){
		RefreshCheck();
	}
	
	//---------------------------------------------------
	// OnApplicationPause()
	// Unity callback function.
	//---------------------------------------------------		
	void OnApplicationPause( bool bPaused ) {
		if ( !bPaused ) {
			// if the game is unpausing, we need to do a check to refresh the mission list	
			RefreshCheck();
		}
	}	
	
	private void RefreshCheck(object sender, EventArgs args){
		RefreshCheck();	
	}
	//---------------------------------------------------
	// RefreshCheck()
	// Function that checks to see if the wellapad missions
	// list should be refreshed.  If it should, it will
	// delete the current save data.
	//---------------------------------------------------		
	public void RefreshCheck() {
        DateTime now = LgDateTime.GetTimeNow();
        TimeSpan sinceCreated = now - DataManager.Instance.GameData.Wellapad.DateMissionsCreated;
		
		// the list needs to be refreshed if it has been more than 12 hours from creation OR the creation time frame (morning/evening)
		// is different than the current time frame (morning/evening)
		bool bRefresh = sinceCreated.TotalHours >= 12 || PlayPeriodLogic.GetTimeFrame( now ) != PlayPeriodLogic.GetTimeFrame( DataManager.Instance.GameData.Wellapad.DateMissionsCreated );
		
		// alert...if the user has not finished the last tutorial, no matter what, don't refresh
		if ( DataManager.Instance.GameData.Tutorial.ListPlayed.Contains( TutorialManager_Bedroom.TUT_LAST ) == false )
			bRefresh = false;

		// if we have to refresh, just delete our data...the missions list will take it from there
		if ( bRefresh ) {
			//Before reseting mission. Go through current mission and send failed tasks to analytics server
			foreach ( KeyValuePair<string, Mission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks ) {
				foreach ( KeyValuePair<string, WellapadTask> taskKeyValuePair in mission.Value.Tasks ) {
					WellapadTask task = taskKeyValuePair.Value;

					//task is incomplete
					if(task.Completed == WellapadTaskCompletionStates.Uncompleted){
						Analytics.Instance.WellapadTaskEvent(Analytics.TASK_STATUS_FAIL, 
							task.MissionID, task.TaskID);
					}
				}
			}

			DataManager.Instance.GameData.Wellapad.ResetMissions();
			
			AddDefaultMissions();
			
			// have the screens of the wellapad refresh before we send out the event below, because we want to make sure the
			// missions screen is active
			// WellapadUIManager.Instance.RefreshScreen();
		
			// send event
			if (OnMissionsRefreshed != null) 
				OnMissionsRefreshed(this, EventArgs.Empty);		
		}
	}
	
	//---------------------------------------------------
	// CreateTutorialMissions()
	// Special behavior called from tutorial -- will
	// destroy the current missions and replace them 
	// with special tutorial missions.
	//---------------------------------------------------		
	public void CreateTutorialMissions() {
		// reset the current data
		DataManager.Instance.GameData.Wellapad.ResetMissions();
		
		AddMission( "TutorialCritical" );
		//AddMission( "TutorialSide" );
		
		// send event
		if ( OnMissionsRefreshed != null ) 
			OnMissionsRefreshed( this, EventArgs.Empty );
	}
	
	//---------------------------------------------------
	// HighlightTask()
	// Sends a message to the UI that will highlight 
	// strTask, and dim out any other tasks.
	//---------------------------------------------------		
	public void HighlightTask( string strTask ) {
		StartCoroutine(HighlightTaskWait(strTask));
	}
	
	// TODO-REFACTOR
	// We wait for 2 frames here because when wellapad is opened, to make sure evrything "OnHighlightTask()" registered from MissionTaskUI
	// Wellapad_MissionList.cs:DisplayMissions() waits a frame already
	// Also take a look at GameTutorial_WellapadIntro.cs:OpeningWellapad(), that waits a frame before calling this!
	//				I think the ^ one can be removed
	private IEnumerator HighlightTaskWait(string strTask){
		yield return 0;
		yield return 0;
		if ( OnHighlightTask != null ) {
			TaskUpdatedArgs args = new TaskUpdatedArgs();
			args.ID = strTask;
			OnHighlightTask( this, args );
		}
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
			Debug.LogError("Reward is attempted to be check for an illegal mission: " + strMissionID);
			return;
		}
		
		// reward is currently unearned
		Mission mission = DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionID];
		if ( mission.RewardStatus != RewardStatuses.Unearned ) {
			Debug.LogError("Reward check revealed illegal state for reward for mission " + strMissionID);
			return;
		}
		
		// loop through all tasks to see their status...if we run into a task that is not completed, just return
		foreach ( KeyValuePair<string, WellapadTask> task in mission.Tasks ) {
			if ( task.Value.Completed != WellapadTaskCompletionStates.RecentlyCompleted && 
					task.Value.Completed != WellapadTaskCompletionStates.Completed )
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
	public WellapadTaskCompletionStates GetTaskStatus( WellapadTask task, bool bPop = false ) {
		WellapadTaskCompletionStates eStatus = WellapadTaskCompletionStates.Uncompleted;
		
		string strMission = task.MissionID;
		string strID = task.TaskID;
		
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strMission ) && 
				DataManager.Instance.GameData.Wellapad.CurrentTasks[strMission].Tasks.ContainsKey( strID ) ) {
			eStatus = DataManager.Instance.GameData.Wellapad.CurrentTasks[strMission].Tasks[strID].Completed;
		
			
			// if the status is recently completed and we are popping, "pop" it by setting it to just plain completed now
			if ( bPop && eStatus == WellapadTaskCompletionStates.RecentlyCompleted )
				DataManager.Instance.GameData.Wellapad.CurrentTasks[strMission].Tasks[strID].Completed = WellapadTaskCompletionStates.Completed;
		}
		else
			Debug.LogError("Can't find task " + strID + " in saved data");
		
		return eStatus;
	}
	
	//---------------------------------------------------
	// GetCurrentMissions()
	//---------------------------------------------------	
	public List<string> GetCurrentMissions() {
		List<string> listMissions = new List<string>();
		
		// if the user does not have any missions saved, give them the default missions
		// probably want to do this through xml data at some point...
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.Count == 0 )
			AddDefaultMissions();
		
		foreach ( KeyValuePair<string, Mission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks ) {
			listMissions.Add( mission.Key );	
		}
		
		return listMissions;
	}
	
	//---------------------------------------------------
	// AddDefaultMissions()
	//---------------------------------------------------		
	private void AddDefaultMissions() {
		AddMission( "Critical" );
		//AddMission( "Side" );		
	}
	
	//---------------------------------------------------
	// GetTasks()
	// Returns a task list for a given mission type.
	// If there are no tasks in save data, it means they
	// must be created first.
	//---------------------------------------------------		
	public List<WellapadTask> GetTasks( string strMissionType ) {
		List<WellapadTask> listTasks = new List<WellapadTask>();
			
		// before creating the missions from scratch, check to see if the user has any save data
		if ( DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey( strMissionType ) ) {
			// user has saved tasks...use those	
			Dictionary<string, WellapadTask> savedTasks = DataManager.Instance.GameData.Wellapad.CurrentTasks[strMissionType].Tasks;
			
			// loop through all saved tasks and add them to the list
			foreach (KeyValuePair<string, WellapadTask> pair in savedTasks)
					listTasks.Add( pair.Value );
		}
		else
			Debug.LogError("Something trying to create a mission in the UI that the user does not have...give it to them first!");

		return listTasks;
	}	
	
	//---------------------------------------------------
	// AddMission()
	//---------------------------------------------------		
	public void AddMission( string strMission ) {
		List<Data_WellapadTask> listTasks = DataLoader_WellapadTasks.GetTasks( strMission );
		Dictionary<string, WellapadTask> savedTasks = new Dictionary<string, WellapadTask>();
		
		for ( int i = 0; i < listTasks.Count; ++i ) {
			Data_WellapadTask task = listTasks[i];
			string strID = task.GetTaskID();
			
			savedTasks[strID] = new WellapadTask( task );
		}
		
		DataManager.Instance.GameData.Wellapad.CurrentTasks[strMission] = new Mission( strMission, savedTasks );
		
		// reset the time -- I probably want to change this to a per mission basis at some point if we expand the system?
		DataManager.Instance.GameData.Wellapad.DateMissionsCreated = LgDateTime.GetTimeNow();
	}	
}
