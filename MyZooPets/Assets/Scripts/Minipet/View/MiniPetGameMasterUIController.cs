﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MiniPetGameMasterUIController : MonoBehaviour {

	//public UILocalize taskLocalize;
	public UILabel label;
	public GameObject rewardButton;
	// tween object for when the task is completed
	public TweenToggle slash;
	public Color tutTextHighlightOn;
	public Color tutTextHighlightOff;
	MutableDataWellapadTask task;

	public void Initialize(string taskID){
		// Not sure what you want to pass in here grab sean to discuss
		List<MutableDataWellapadTask> listTasks = WellapadMissionController.Instance.GetTasks(taskID); 
		task = listTasks[0];
		ImmutableDataWellapadTask stuff = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc = stuff.GetText();
		rewardButton.GetComponent<LgButtonMessage>().target = MiniPetManager.Instance.MiniPetTable["MiniPet1"];
		if(task.Amount > 0){
			desc = String.Format(desc, task.Amount);
		}
		label.text = desc;
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
		Debug.Log("=== Checking status of checkbox ===");
		Debug.Log("uncompleted " + (eStatus == WellapadTaskCompletionStates.Uncompleted));
		Debug.Log("complete " + (eStatus == WellapadTaskCompletionStates.Completed));
		Debug.Log("recently completed " + (eStatus == WellapadTaskCompletionStates.RecentlyCompleted));
		Debug.Log("===                             ===");
		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if(eStatus == WellapadTaskCompletionStates.Completed ||
		   (eStatus == WellapadTaskCompletionStates.RecentlyCompleted && bPop)){
			// mark this task as done
			slash.gameObject.SetActive(true);
			StartCoroutine(CheckboxSpriteShowHelper());	// Show after one frame
			rewardButton.SetActive(true);
		}
	}

	private IEnumerator CheckboxSpriteShowHelper(){
		yield return 0;
		slash.Show();
	}
	//---------------------------------------------------
	// OnTweenDone()
	// Callback for when the wellapad UI is done tweening.
	//---------------------------------------------------		
	private void OnTweenDone(object sender, UIManagerEventArgs args){
		// if the UI is opening, update our task
		if(args.Opening)
			SetCheckboxSprite(true);
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	void OnDestroy(){
		if(WellapadUIManager.Instance){
			WellapadUIManager.Instance.OnTweenDone -= OnTweenDone;
		}
	}
}
