using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadMissionController
// Controller class for the wellapad's missions.
//---------------------------------------------------

// event args for OnTaskUpdated event
public class TaskUpdatedArgs : EventArgs{
	public string ID{ get; set; }
	public string Mission{ get; set; }
	public bool Status{ get; set; }
}

public class WellapadMissionController : Singleton<WellapadMissionController>{
	//=======================Events========================
	public EventHandler<TaskUpdatedArgs> OnTaskUpdated;   	// when a task's status is updated
	public EventHandler<EventArgs> OnMissionsRefreshed;		// when missions get refreshed
	public EventHandler<EventArgs> OnRewardClaimed;			// when a reward is claimed
	//=====================================================

	//check missions based off this
	public bool needMission{get; set;}

	/// <summary>
	/// Gets the mission.
	/// </summary>
	/// <returns>The mission.</returns>
	/// <param name="missionID">Mission ID.</param>
	public MutableDataMission GetMission(string missionID){
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(missionID))
			return DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID];
		else{
			Debug.LogError("No such mission in current tasks: " + missionID);
			return null;
		}
	}

	/// <summary>
	/// This function will return true if at least one
	/// mission the user has is either unclaimed or
	/// unearned reward.
	/// </summary>
	/// <returns><c>true</c> if this instance has active tasks; otherwise, <c>false</c>.</returns>
	public bool HasActiveTasks(){
		// start off assuming inactive
		bool isActive = false;
		
		// loop through all missions and check their reward status
		foreach(KeyValuePair<string, MutableDataMission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks){
			MutableDataMission thisMission = mission.Value;
			if(thisMission.RewardStatus == RewardStatuses.Unearned || thisMission.RewardStatus == RewardStatuses.Unclaimed)
				isActive = true;
		}

		return isActive;
	}

	/// <summary>
	/// Unlocks the task.
	/// Marks the incoming task as unlocked -- it may now be eligible for being
	/// in a mission
	/// </summary>
	/// <param name="taskID">Task ID.</param>
	public void UnlockTask(string taskID){
		DataManager.Instance.GameData.Wellapad.TasksUnlocked.Add(taskID);

	}

	/// <summary>
	/// Claims the reward. Called when user successfully claimed reward from a mission
	/// </summary>
	/// <param name="missionID">Mission ID.</param>
	public void ClaimReward(string missionID, GameObject rewardObject = null){
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(missionID)){
			// one final check just to be safe
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID].RewardStatus == RewardStatuses.Unclaimed){

				HUDUIManager.Instance.ShowPanel();

				// get the position of the actual reward object because we want to stream the XP from it
				if(rewardObject != null){
					Vector3 screenPos = LgNGUITools.GetScreenPosition(rewardObject);
					screenPos = CameraManager.Instance.TransformAnchorPosition(screenPos, InterfaceAnchors.TopLeft, InterfaceAnchors.Top);
					StatsController.Instance.ChangeStats(deltaStars: 50, starsLoc: screenPos);
				}
				else{
					StatsController.Instance.ChangeStats(deltaStars: 50);
				}

				DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID].RewardStatus = RewardStatuses.Claimed;

				//Send analytics event
				Analytics.Instance.ClaimWellapadReward();
				
				if(OnRewardClaimed != null){
					OnRewardClaimed(this, EventArgs.Empty);
				}
				
//				Debug.Log("Reward claimed for mission: " + strMissionID);
			}
			else{
				Debug.LogError("Something trying to claim an unclaimable reward for mission: " + missionID);
			}
		}
		else{
			Debug.LogError("Something trying to claim a reward for non-current mission: " + missionID);
		}
	}

	/// <summary>
	/// Called from various parts of the game when a task is completed that may
	/// be an active mission for the player.
	/// </summary>
	/// <param name="completedTaskID">Completed task ID.</param>
	/// <param name="completeCondition">Complete condition.</param>
	public void TaskCompleted(string completedTaskID, int completeCondition = 0){
		// check to see if the completed task was in a player's mission -- if it was, mark it was done
		foreach(KeyValuePair<string, MutableDataMission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks){
			foreach(KeyValuePair<string, MutableDataWellapadTask> task in mission.Value.Tasks){
				if(task.Value.WillComplete(completedTaskID, completeCondition)){
//					Debug.Log("++ valid task completed and set: " + completedTaskID);

					string taskID = task.Value.TaskID;
					DataManager.Instance.GameData.Wellapad.CurrentTasks[mission.Key].Tasks[taskID].Completed = WellapadTaskCompletionStates.RecentlyCompleted;

					// because the task was completed, we may have to update our reward status...
					// got to do this before sending the event or the reward will not be displayed right
					RewardCheck(mission.Key);						

					//Analytics
					Analytics.Instance.WellapadTaskEvent(Analytics.TASK_STATUS_COMPLETE, task.Value.MissionID, task.Value.TaskID);
					
					// send event for the task update
					if(OnTaskUpdated != null){
						TaskUpdatedArgs args = new TaskUpdatedArgs();
						args.Status = true;
						args.ID = completedTaskID;
						args.Mission = mission.Key;
						OnTaskUpdated(this, args);
					}
					
					// right now there will only be unique tasks for each mission, so if we've found it, just return out
					return;
				}
			}
		}
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			// if the game is unpausing, we need to do a check to refresh the mission list	
			RefreshCheck();
		}
	}

	/// <summary>
	/// Refreshs check.
	/// Function that checks to see if the wellapad missions
	/// list should be refreshed.  If it should, it will
	/// delete the current save data.
	/// </summary>
	public void RefreshCheck(){
		//do not refresh wellapad task for lite version
		// if(VersionManager.IsLite()) return;

		DateTime now = LgDateTime.GetTimeNow();
		TimeSpan sinceCreated = now - DataManager.Instance.GameData.Wellapad.DateMissionsCreated;
		
		// the list needs to be refreshed if it has been more than 12 hours from creation OR the creation time frame (morning/evening)
		// is different than the current time frame (morning/evening)
		DateTime dateMissionsCreated = DataManager.Instance.GameData.Wellapad.DateMissionsCreated;	
		bool IsRefresh = sinceCreated.TotalHours >= 12 || 
			PlayPeriodLogic.GetTimeFrame(now) != PlayPeriodLogic.GetTimeFrame(dateMissionsCreated);
		//bool IsRefresh = needMission;
		// alert...if the user has not finished the last tutorial, no matter what, don't refresh
		/*if(!DataManager.Instance.GameData.Tutorial.AreTutorialsFinished())
			IsRefresh = false;*/

		// if we have to refresh, just delete our data...the missions list will take it from there
		if(IsRefresh){
			//Before reseting mission. Go through current mission and send failed tasks to analytics server
			foreach(KeyValuePair<string, MutableDataMission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks){
				foreach(KeyValuePair<string, MutableDataWellapadTask> taskKeyValuePair in mission.Value.Tasks){
					MutableDataWellapadTask task = taskKeyValuePair.Value;

					//task is incomplete
					if(task.Completed == WellapadTaskCompletionStates.Uncompleted){
						Analytics.Instance.WellapadTaskEvent(Analytics.TASK_STATUS_FAIL, 
							task.MissionID, task.TaskID);
						if(task.TaskID == "DailyInhaler"){
							Analytics.Instance.DidUseInhaler(false);
						}
					}
				}
			}

			DataManager.Instance.GameData.Wellapad.ResetMissions();
			
			//AddDefaultMissions();
			
			// have the screens of the wellapad refresh before we send out the event below, because we want to make sure the
			// missions screen is active
			//WellapadUIManager.Instance.RefreshScreen();
		
			// send event
			if(OnMissionsRefreshed != null) 
				OnMissionsRefreshed(this, EventArgs.Empty);		
			needMission = false;
			IsRefresh = false;
		}
	}

	/// <summary>
	/// Creates the tutorial part1 missions.
	/// Special behavior called from tutorial -- will
	/// destroy the current missions and replace them 
	/// with special tutorial missions.
	/// </summary>
	public void CreateTutorialPart1Missions(){
		// reset the current data
		DataManager.Instance.GameData.Wellapad.ResetMissions();
		
		AddMission("TutorialPart1");
		
		// send event
		if(OnMissionsRefreshed != null) 
			OnMissionsRefreshed(this, EventArgs.Empty);
	}

	public void CreateTutorialPart2Missions(){
		// reset the current data
		DataManager.Instance.GameData.Wellapad.ResetMissions();
		
		AddMission("TutorialPart2");
		
		// send event
		if(OnMissionsRefreshed != null) 
			OnMissionsRefreshed(this, EventArgs.Empty);
	}

	/// <summary>
	/// This function was called because some task in
	/// missionID was just completed, so we need to see
	/// if that mission's reward needs a status update.  This
	/// is kind of heavy handed, but I'm not sure how else
	/// to do it at the moment.
	/// Note that this doesn't actually effect the UI...it
	/// just keeps our internal state correct.  It's done
	/// this way because our save system needs ot save
	/// primitives.
	/// </summary>
	/// <param name="missionID">Mission ID.</param>
	private void RewardCheck(string missionID){
		// do some legality checks

		// mission exists
		if(!DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(missionID)){
			Debug.LogError("Reward is attempted to be check for an illegal mission: " + missionID);
			return;
		}
		
		// reward is currently unearned
		MutableDataMission mission = DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID];
		if(mission.RewardStatus != RewardStatuses.Unearned){
			Debug.LogError("Reward check revealed illegal state for reward for mission " + missionID);
			return;
		}
		
		// loop through all tasks to see their status...if we run into a task that is not completed, just return
		foreach(KeyValuePair<string, MutableDataWellapadTask> task in mission.Tasks){
			if(task.Value.Completed != WellapadTaskCompletionStates.RecentlyCompleted && 
				task.Value.Completed != WellapadTaskCompletionStates.Completed)
				return;
		}
		
		// if we get here it means that all tasks in the mission are complete -- the reward is now unclaimed
		mission.RewardStatus = RewardStatuses.Unclaimed;
	}

	/// <summary>
	/// Gets the task status. Returns whether the user has completed the incoming
	/// task or not.
	/// </summary>
	/// <returns>The task status.</returns>
	/// <param name="task">Task.</param>
	public WellapadTaskCompletionStates GetTaskStatus(MutableDataWellapadTask task, bool bPop = false){
		WellapadTaskCompletionStates status = WellapadTaskCompletionStates.Uncompleted;
		
		string missionID = task.MissionID;
		string taskID = task.TaskID;
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(missionID) && 
			DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID].Tasks.ContainsKey(taskID)){

			status = DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID].Tasks[taskID].Completed;
			
			// if the status is recently completed and we are popping, "pop" it by setting it to just plain completed now
			if(bPop && status == WellapadTaskCompletionStates.RecentlyCompleted){
				DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID].Tasks[taskID].Completed = WellapadTaskCompletionStates.Completed;
			}
		}
		else{
			Debug.LogError("Can't find task " + taskID + " in saved data");
		}
		
		return status;
	}

	public List<string> GetCurrentMissions(){
		List<string> listMissions = new List<string>();
		// if the user does not have any missions saved, give them the default missions
		// probably want to do this through xml data at some point...
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.Count == 0){
			//AddDefaultMissions();
		}
		foreach(KeyValuePair<string, MutableDataMission> mission in DataManager.Instance.GameData.Wellapad.CurrentTasks){
			listMissions.Add(mission.Key);	
		}
		
		return listMissions;
	}

	private void AddDefaultMissions(){
		AddMission("Critical");
	}

	/// <summary>
	/// Gets the tasks. Returns a task list for a given mission type. If there are
	/// no tasks in save data, it means they must be created first
	/// </summary>
	/// <returns>The tasks.</returns>
	/// <param name="missionID">Mission ID.</param>
	public List<MutableDataWellapadTask> GetTasks(string missionID){
		List<MutableDataWellapadTask> listTasks = new List<MutableDataWellapadTask>();
		// before creating the missions from scratch, check to see if the user has any save data
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(missionID)){
			// user has saved tasks...use those	
			Dictionary<string, MutableDataWellapadTask> savedTasks = DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID].Tasks;
			// loop through all saved tasks and add them to the list
			foreach(KeyValuePair<string, MutableDataWellapadTask> pair in savedTasks){
				listTasks.Add(pair.Value);
			}
		}
		else{
			Debug.LogError("Something trying to create a mission in the UI that the user does not have...give it to them first!");
		}
		return listTasks;
	}	

	public void AddMission(string missionID){
		List<ImmutableDataWellapadTask> listTasks = GetUnlockedTasks(missionID);
		Dictionary<string, MutableDataWellapadTask> savedTasks = new Dictionary<string, MutableDataWellapadTask>();
		for(int i = 0; i < listTasks.Count; ++i){
			ImmutableDataWellapadTask task = listTasks[i];
			string taskID = task.GetTaskID();
			savedTasks[taskID] = new MutableDataWellapadTask(task);
		}
		DataManager.Instance.GameData.Wellapad.CurrentTasks[missionID] = new MutableDataMission(missionID, savedTasks);
		
		// reset the time -- I probably want to change this to a per mission basis at some point if we expand the system?
		DataManager.Instance.GameData.Wellapad.DateMissionsCreated = LgDateTime.GetTimeNow();
		// send event
		if(OnMissionsRefreshed != null){
			OnMissionsRefreshed(this, EventArgs.Empty);
		}
	}

	private List<ImmutableDataWellapadTask> GetUnlockedTasks(string missionID){
		Hashtable taskHash = DataLoaderWellapadTasks.GetTasks(missionID);
		List<ImmutableDataWellapadTask> taskListFinal = new List<ImmutableDataWellapadTask>();
		// now go through each category in this hash and pick one task at random and add it to our list of tasks
		// (but also check to make sure the category is unlocked)
		foreach(DictionaryEntry pair in taskHash){
			string category = (string)pair.Key;
			if(DataManager.Instance.GameData.Wellapad.TasksUnlocked.Contains(category)){
				List<ImmutableDataWellapadTask> listTasks = (List<ImmutableDataWellapadTask>) pair.Value;
				// get a random number of tasks to add to the list -- if the category is "Always" we want all the tasks,
				// otherwise we just want to pick 1 at random
				// int nTasks = strCategory == WellapadData.ALWAYShashDataD ? listTasks.Count : 1;
				int numberOfTasks = category == "Always" ? listTasks.Count : 1;
				
				// this is a little weird...the random element thing is messing up the ordering of the tasks
				List<ImmutableDataWellapadTask> tasks = listTasks;
				if(numberOfTasks != listTasks.Count){
					tasks = ListUtils.GetRandomElements<ImmutableDataWellapadTask>(listTasks, numberOfTasks);
				}
				
				// add each of our tasks to the final list
				foreach(ImmutableDataWellapadTask task in tasks){
					taskListFinal.Add(task);
				}
			}
		}
		return taskListFinal;
	}
}
