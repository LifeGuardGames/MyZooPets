using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetGameMasterUIController : MonoBehaviour {

	public UILocalize taskLocalize;

	public void Initialize(string taskID){
		// Not sure what you want to pass in here grab sean to discuss
		//List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(taskID);
		//MutableDataWellapadTask task = listTasks[0];
		//ImmutableDataWellapadTask stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		taskID = "Task_ScoreNinja";
		taskLocalize.key = taskID;
		taskLocalize.Localize();
	}
}
