using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mutable data mission.
/// </summary>
public class MutableDataTaskList{
	public Dictionary<string, MutableDataWellapadTask> Tasks { get; set; } //key: TaskID, Value: MutableDataWellapadTask

	public MutableDataTaskList(){}

	public MutableDataTaskList(Dictionary<string, MutableDataWellapadTask> tasks){
		Tasks = tasks;
	}
}

