using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mutable data mission.
/// </summary>
public class MutableDataMission{
	public string ID { get; set; }						// id of the mission
	public RewardStatuses RewardStatus { get; set; }		// status of the mission's reward
	public Dictionary<string, MutableDataWellapadTask> Tasks { get; set; } //key: TaskID, Value: MutableDataWellapadTask

	public MutableDataMission(){}

	public MutableDataMission(string id, Dictionary<string, MutableDataWellapadTask> tasks, 
	                          RewardStatuses reward = RewardStatuses.Unearned){
		ID = id;
		Tasks = tasks;
		RewardStatus = reward;
	}
}

