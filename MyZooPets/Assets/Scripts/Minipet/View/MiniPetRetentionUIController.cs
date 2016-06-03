using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MiniPetRetentionUIController : MonoBehaviour {

	public Text mission1Localize;
	public Text mission2Localize;
	public Text mission3Localize;
	public Text mission4Localize;
	MutableDataWellapadTask task;
	public TweenToggle slash1;
	public TweenToggle slash2;
	public TweenToggle slash3;
	public TweenToggle slash4;
	public GameObject rewardButton;

	private MiniPetRetentionPet retentionScript;	// Reference to minipet logic

	public void InitializeContent(string taskID, MiniPetRetentionPet retentionScript){
		this.retentionScript = retentionScript;
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")){
				List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTaskGroup("TutorialPart1"); 
				task = listTasks[0];
				ImmutableDataWellapadTask missionTask = DataLoaderWellapadTasks.GetTask(task.TaskID);
				SetCheckboxSprite(true, slash1);
				task = listTasks[1];
				SetCheckboxSprite(true, slash2);
				ImmutableDataWellapadTask missionTask2 = DataLoaderWellapadTasks.GetTask(task.TaskID);
				mission1Localize.text = "Task_" + missionTask.GetTaskID().ToString();
				mission2Localize.text = "Task_" + missionTask2.GetTaskID().ToString();
				mission3Localize.gameObject.SetActive(false);
				mission4Localize.gameObject.SetActive(false);
				if(WellapadMissionController.Instance.GetTaskStatus(listTasks[0]) == WellapadTaskCompletionStates.Completed
			   		&& WellapadMissionController.Instance.GetTaskStatus(listTasks[1])  == WellapadTaskCompletionStates.Completed){
					rewardButton.SetActive(true);
					rewardButton.GetComponent<LgButtonMessage>().target = MiniPetManager.Instance.MiniPetTable["MiniPet0"];
				}
			}
			else {
//			Debug.Log(DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].RewardStatus);
			if(DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].isReward == RewardStatuses.Unclaimed
			   || DataManager.Instance.GameData.Wellapad.CurrentTasks[taskID].isReward == RewardStatuses.Unearned){
					//Debug.Log(taskID);
					List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTaskGroup("Critical"); 
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
					mission1Localize.text = "Task_" + missionTask.GetTaskID().ToString();
					mission2Localize.text = "Task_" + missionTask2.GetTaskID().ToString();
					mission3Localize.text = "Task_" + missionTask3.GetTaskID();
					mission4Localize.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// Sets the sprite on this UI's checkbox based on the status of the task.
	/// </summary>
	private void SetCheckboxSprite(bool isPop, TweenToggle slash){
		// get the status
		WellapadTaskCompletionStates eStatus = WellapadMissionController.Instance.GetTaskStatus(task, isPop);
		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if(eStatus == WellapadTaskCompletionStates.Completed ||
		   (eStatus == WellapadTaskCompletionStates.RecentlyCompleted && isPop)){
			// mark this task as done
			slash.gameObject.SetActive(true);
			StartCoroutine(CheckboxSpriteShowHelper(slash));	// Show after one frame
		}
	}

	private IEnumerator CheckboxSpriteShowHelper(TweenToggle slash){
		yield return 0;
		slash.Show();
	}

	public GameObject GetRewardButton(){
		return rewardButton;
	}

	public void HideRewardButton(){
		rewardButton.SetActive(false);
	}
}
