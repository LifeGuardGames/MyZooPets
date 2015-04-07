using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MiniPetRetentionUIController : MonoBehaviour {

	public UILocalize mission1Localize;
	public UILocalize mission2Localize;
	public UILocalize mission3Localize;
	MutableDataWellapadTask task;
	public TweenToggle slash1;
	public TweenToggle slash2;
	public TweenToggle slash3;
	public GameObject rewardButton;


	public void Initialize(){
		string taskID = "Critical";
		List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(taskID); 
		task = listTasks[0];
		SetCheckboxSprite(true, slash1);
		ImmutableDataWellapadTask missionTask = DataLoaderWellapadTasks.GetTask(task.TaskID);
		rewardButton.GetComponent<LgButtonMessage>().target = MiniPetManager.Instance.MiniPetTable["MiniPet0"];
		task = listTasks[1];
		SetCheckboxSprite(true, slash2);
		ImmutableDataWellapadTask missionTask2 = DataLoaderWellapadTasks.GetTask(task.TaskID);
		task = listTasks[2];
		SetCheckboxSprite(true,slash3);
		ImmutableDataWellapadTask missionTask3 = DataLoaderWellapadTasks.GetTask(task.TaskID);
		if(WellapadMissionController.Instance.GetTaskStatus(listTasks[0]) == WellapadTaskCompletionStates.Completed &&WellapadMissionController.Instance.GetTaskStatus(listTasks[1])  == WellapadTaskCompletionStates.Completed&&WellapadMissionController.Instance.GetTaskStatus(listTasks[2]) == WellapadTaskCompletionStates.Completed ){
			rewardButton.SetActive(true);
		}
		Debug.Log("Task_"+missionTask.GetTaskID());
		Debug.Log(mission1Localize.key);
		mission1Localize.key = "Task_"+missionTask.GetTaskID().ToString();
		mission1Localize.Localize();
		mission2Localize.key = "Task_"+missionTask2.GetTaskID();
		mission2Localize.Localize();
		mission3Localize.key = "Task_"+missionTask3.GetTaskID();
		mission3Localize.Localize();
	}

	//---------------------------------------------------
	// SetCheckboxSprite()
	// Sets the sprite on this UI's checkbox based on
	// the status of the task.
	//---------------------------------------------------	
	private void SetCheckboxSprite(bool bPop, TweenToggle slash){
		// get the status
		WellapadTaskCompletionStates eStatus = WellapadMissionController.Instance.GetTaskStatus(task, bPop);
		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if(eStatus == WellapadTaskCompletionStates.Completed ||
		   (eStatus == WellapadTaskCompletionStates.RecentlyCompleted && bPop)){
			// mark this task as done
			slash.gameObject.SetActive(true);
			StartCoroutine(CheckboxSpriteShowHelper(slash));	// Show after one frame

		}
	}

	private IEnumerator CheckboxSpriteShowHelper(TweenToggle slash){
		yield return 0;
		slash.Show();
	}

}
