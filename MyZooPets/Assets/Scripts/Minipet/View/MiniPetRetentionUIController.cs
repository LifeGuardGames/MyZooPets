using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MiniPetRetentionUIController : MonoBehaviour {

	public UILocalize missionLocalize;
	public UILabel mission1;
	public UILabel mission2;
	public UILabel mission3;
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
		ImmutableDataWellapadTask stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc = stuff.GetText();
		rewardButton.GetComponent<LgButtonMessage>().target = MiniPetManager.Instance.MiniPetTable["MiniPet0"];
		task = listTasks[1];
		SetCheckboxSprite(true, slash2);
		stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc2 = stuff.GetText();
		task = listTasks[2];
		SetCheckboxSprite(true,slash3);
		stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc3 = stuff.GetText();
		if(WellapadMissionController.Instance.GetTaskStatus(listTasks[0]) == WellapadTaskCompletionStates.Completed &&WellapadMissionController.Instance.GetTaskStatus(listTasks[1])  == WellapadTaskCompletionStates.Completed&&WellapadMissionController.Instance.GetTaskStatus(listTasks[2]) == WellapadTaskCompletionStates.Completed ){
			rewardButton.SetActive(true);
		}
		mission1.text = desc;
		mission2.text = desc2;
		mission3.text = desc3;
	}

	//---------------------------------------------------
	// SetCheckboxSprite()
	// Sets the sprite on this UI's checkbox based on
	// the status of the task.
	//---------------------------------------------------	
	private void SetCheckboxSprite(bool bPop, TweenToggle slash){
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
			StartCoroutine(CheckboxSpriteShowHelper(slash));	// Show after one frame

		}
	}

	private IEnumerator CheckboxSpriteShowHelper(TweenToggle slash){
		yield return 0;
		slash.Show();
	}

}
