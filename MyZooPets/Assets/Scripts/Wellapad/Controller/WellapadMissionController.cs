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

	/// <summary>
	/// Gets the mission.
	/// </summary>
	/// <returns>The mission.</returns>
	/// <param name="taskID">Mission ID.</param>
	public MutableDataWellapadTask GetTask(string taskID){
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(taskID))
			return DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID];
		else{
			Debug.LogError("No such mission in current tasks: " + taskID);
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
		foreach(KeyValuePair<string, MutableDataWellapadTask> task in DataManager.Instance.GameData.Wellapad.CurrentTasks){
			MutableDataWellapadTask thisTask = task.Value;
			if(thisTask.isReward == RewardStatuses.Unearned || thisTask.isReward == RewardStatuses.Unclaimed)
				isActive = true;
		}

		return isActive;
	}


	/// <summary>
	/// Claims the reward. Called when user successfully claimed reward from a mission
	/// </summary>
	/// <param name="taskID">Mission ID.</param>
	public void ClaimReward(string taskID, GameObject rewardObject = null){
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(taskID)){
			// one final check just to be safe
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].isReward == RewardStatuses.Unclaimed){

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

				DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].isReward = RewardStatuses.Claimed;

				//Send analytics event
				Analytics.Instance.ClaimWellapadReward();
				
				if(OnRewardClaimed != null){
					OnRewardClaimed(this, EventArgs.Empty);
				}
				
//				Debug.Log("Reward claimed for mission: " + strMissionID);
			}
			else{
				Debug.LogError("Something trying to claim an unclaimable reward for mission: " + taskID);
			}
		}
		else{
			Debug.LogError("Something trying to claim a reward for non-current mission: " + taskID);
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
		foreach(KeyValuePair<string, MutableDataWellapadTask> _task in DataManager.Instance.GameData.Wellapad.CurrentTasks){
				if(_task.Value.WillComplete(completedTaskID, completeCondition)){
//					Debug.Log("++ valid task completed and set: " + completedTaskID);

				string taskID = _task.Value.TaskID;
				DataManager.Instance.GameData.Wellapad.CurrentTasks[_task.Key].Completed = WellapadTaskCompletionStates.RecentlyCompleted;

				// because the task was completed, we may have to update our reward status...
				// got to do this before sending the event or the reward will not be displayed right
				RewardCheck(_task.Key);						

				//Analytics
				Analytics.Instance.WellapadTaskEvent(Analytics.TASK_STATUS_COMPLETE, _task.Value.TaskID, _task.Value.TaskID);
					
				// send event for the task update
				if(OnTaskUpdated != null){
					TaskUpdatedArgs args = new TaskUpdatedArgs();
					args.Status = true;
					args.ID = completedTaskID;
					args.Mission = _task.Key;
					OnTaskUpdated(this, args);
				}
					
				// right now there will only be unique tasks for each mission, so if we've found it, just return out
				return;
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
			foreach(KeyValuePair<string, MutableDataWellapadTask> taskList in DataManager.Instance.GameData.Wellapad.CurrentTasks){
			
				MutableDataWellapadTask _task = taskList.Value;

				//task is incomplete
				if(_task.Completed == WellapadTaskCompletionStates.Uncompleted){
					Analytics.Instance.WellapadTaskEvent(Analytics.TASK_STATUS_FAIL, 
						_task.TaskID, _task.TaskID);
					if(_task.TaskID == "DailyInhaler"){
						Analytics.Instance.DidUseInhaler(false);
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

		AddTask("DailyInhaler");
		AddTask("FightMonster");


		// send event
		if(OnMissionsRefreshed != null) 
			OnMissionsRefreshed(this, EventArgs.Empty);
	}

	public void CreateTutorialPart2Missions(){
		// reset the current data
		DataManager.Instance.GameData.Wellapad.ResetMissions();

		AddTask("DailyInhaler");
		AddTask("FightMonster");
		AddTask("CleanRoom");

		// send event
		if(OnMissionsRefreshed != null) 
			OnMissionsRefreshed(this, EventArgs.Empty);
	}

	public RewardStatuses CheckGroupReward() {

		foreach(KeyValuePair<string, MutableDataWellapadTask> _task in DataManager.Instance.GameData.Wellapad.CurrentTasks) {
			if(_task.Value.Category == MiniGameCategory.Critical) {
				if(_task.Value.isReward == RewardStatuses.Unearned) {
					return RewardStatuses.Unearned;
				}
				else {
					return RewardStatuses.Unclaimed;
				}
			}
		}
		return RewardStatuses.Claimed;
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
	/// <param name="taskID">Mission ID.</param>
	private void RewardCheck(string taskID){
		// do some legality checks

		// mission exists
		if(!DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(taskID)){
			Debug.LogError("Reward is attempted to be check for an illegal mission: " + taskID);
			return;
		}

		// reward is currently unearned
		MutableDataWellapadTask task = DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID];
		if(task.Category == MiniGameCategory.Critical) {
			foreach(KeyValuePair<string, MutableDataWellapadTask> _task in DataManager.Instance.GameData.Wellapad.CurrentTasks) {
				if(_task.Value.Category == MiniGameCategory.Critical) {
					if(_task.Value.isReward == RewardStatuses.Unearned) {
						return;
					}
				}
			}
		}
		else {
			if(task.isReward != RewardStatuses.Unearned) {
				Debug.LogError("Reward check revealed illegal state for reward for mission " + taskID);
				return;
			}
		}
		
		// if we get here it means that all tasks in the mission are complete -- the reward is now unclaimed
		task.isReward = RewardStatuses.Unclaimed;
	}

	/// <summary>
	/// Gets the task status. Returns whether the user has completed the incoming
	/// task or not.
	/// </summary>
	/// <returns>The task status.</returns>
	/// <param name="task">Task.</param>
	public WellapadTaskCompletionStates GetTaskStatus(MutableDataWellapadTask task, bool bPop = false){
		WellapadTaskCompletionStates status = WellapadTaskCompletionStates.Uncompleted;
		
		string taskID = task.TaskID;
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(taskID) && 
			DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(taskID)){

			status = DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].Completed;
			
			// if the status is recently completed and we are popping, "pop" it by setting it to just plain completed now
			if(bPop && status == WellapadTaskCompletionStates.RecentlyCompleted){
				DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].Completed = WellapadTaskCompletionStates.Completed;
			}
		}
		else{
			Debug.LogError("Can't find task " + taskID + " in saved data");
		}
		
		return status;
	}


	/// <summary>
	/// Gets the tasks. Returns a task list for a given mission type. If there are
	/// no tasks in save data, it means they must be created first
	/// </summary>
	/// <returns>The tasks.</returns>
	public List<string> GetCurrentTasks(){
		List<string> listTask = new List<string>();
		// if the user does not have any missions saved, give them the default missions
		// probably want to do this through xml data at some point...
		if(DataManager.Instance.GameData.Wellapad.CurrentTasks.Count == 0){
			//AddDefaultMissions();
		}
		foreach(MutableDataWellapadTask task in DataManager.Instance.GameData.Wellapad.CurrentTasks.Values){
			listTask.Add(task.TaskID);	
		}
		
		return listTask;
	}

	private void AddDefaultMissions(){
		AddTask("DailyInhaler");
		AddTask("FightMonster");
	}


	public void AddTask(string taskID){
		ImmutableDataWellapadTask task = DataLoaderWellapadTasks.GetTask(taskID);
		DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID] = new MutableDataWellapadTask(task);
		
		// reset the time -- I probably want to change this to a per mission basis at some point if we expand the system?
		DataManager.Instance.GameData.Wellapad.DateMissionsCreated = LgDateTime.GetTimeNow();
		// send event
		if(OnMissionsRefreshed != null){
			OnMissionsRefreshed(this, EventArgs.Empty);
		}
	}

	public List<MutableDataWellapadTask> GetTaskGroup(string missionID) {
		List<MutableDataWellapadTask> listTask = new List<MutableDataWellapadTask>();
		if(missionID == "TutorialPart1") {
			MutableDataWellapadTask task = new MutableDataWellapadTask(DataLoaderWellapadTasks.GetTask("DailyInhaler"));
            listTask.Add(task);
			MutableDataWellapadTask task2 = new MutableDataWellapadTask(DataLoaderWellapadTasks.GetTask("FightMonster"));
			listTask.Add(task2);
		}
		if(missionID == "Critical") {
			foreach(MutableDataWellapadTask task in DataManager.Instance.GameData.Wellapad.CurrentTasks.Values) {
				if(task.Category == MiniGameCategory.Critical) {
					listTask.Add(task);
				}
			}
		}
		return listTask;
	}
}
