using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MiniPetRetentionUIController : MonoBehaviour {

	public UILocalize missionLocalize;
	public UILabel mission;
	MutableDataWellapadTask task;
	public TweenToggle slash;
	public Color tutTextHighlightOn;
	public Color tutTextHighlightOff;
	public GameObject rewardButton;

	public void Initialize(){
		string taskID = "Critical";
		List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(taskID); 
		task = listTasks[0];
		ImmutableDataWellapadTask stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc = stuff.GetText();

		task = listTasks[1];
		stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc2 = stuff.GetText();
		task = listTasks[2];
		stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc3 = stuff.GetText();
		if(WellapadMissionController.Instance.GetTaskStatus(listTasks[0]) == WellapadTaskCompletionStates.Completed &&WellapadMissionController.Instance.GetTaskStatus(listTasks[1])  == WellapadTaskCompletionStates.Completed&&WellapadMissionController.Instance.GetTaskStatus(listTasks[2]) == WellapadTaskCompletionStates.Completed ){
			rewardButton.SetActive(true);
		}
		mission.text = desc + "\n" + "\n" + desc2+ "\n" + "\n" + desc3;
		SetCheckboxSprite(true);
	}

	//---------------------------------------------------
	// SetCheckboxSprite()
	// Sets the sprite on this UI's checkbox based on
	// the status of the task.
	//---------------------------------------------------	
	private void SetCheckboxSprite(bool bPop){
		// get the status
		WellapadTaskCompletionStates eStatus = WellapadMissionController.Instance.GetTaskStatus(task, bPop);
		Debug.Log(eStatus == WellapadTaskCompletionStates.Uncompleted);
		Debug.Log(eStatus == WellapadTaskCompletionStates.Completed);
		Debug.Log(eStatus == WellapadTaskCompletionStates.RecentlyCompleted);
		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if(eStatus == WellapadTaskCompletionStates.Completed ||
		   (eStatus == WellapadTaskCompletionStates.RecentlyCompleted && bPop)){
			// mark this task as done
			slash.gameObject.SetActive(true);
			StartCoroutine(CheckboxSpriteShowHelper());	// Show after one frame

		}
	}
	private IEnumerator CheckboxSpriteShowHelper(){
		yield return 0;
		slash.Show();
	}
}
