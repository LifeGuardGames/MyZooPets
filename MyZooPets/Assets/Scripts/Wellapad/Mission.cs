using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// this is save date for a given wellapad mission
// this was taken from WellapadMissionController, I didn't want to refactor it just yet though.
// This is mutable save data.

public class Mission {
    public string ID {get; set;}						// id of the mission
	public RewardStatuses RewardStatus {get; set;}		// status of the mission's reward
	public Dictionary<string, WellapadTask> Tasks {get; set;}	// all the tasks and their status for this mission

    public Mission(){}

    public Mission(string id, Dictionary<string, WellapadTask> tasks, RewardStatuses eReward = RewardStatuses.Unearned ){
        ID = id;
		Tasks = tasks;
		RewardStatus = eReward;
    }
}

