using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MiniPetGameMasterUIController : MonoBehaviour{
	public Text label;
	public Image spriteIcon;
	public GameObject rewardButton;

	// tween object for when the task is completed
	public TweenToggle checkTween;
	MutableDataWellapadTask task;

	private MiniPetGameMaster gameMasterScript;		// Reference to minipet logic

	public void InitializeContent(string taskID, MinigameTypes type, MiniPetGameMaster gameMasterScript){
		this.gameMasterScript = gameMasterScript;
		task = WellapadMissionController.Instance.GetTask(taskID); 
		ImmutableDataWellapadTask missionTask = DataLoaderWellapadTasks.GetTask(task.TaskID);
		string desc = missionTask.GetText();
		if(task.Amount > 0){
			desc = String.Format(desc, task.Amount);
		}
		label.text = desc;
		spriteIcon.sprite = SpriteCacheManager.GetSprite("mapIcons" + type.ToString());
		rewardButton.SetActive(false);
		SetCheckboxSprite(true);
	}

	/// <summary>
	/// Sets the sprite on this UI's checkbox based on the status of the task.
	/// </summary>
	private void SetCheckboxSprite(bool bPop){
		// get the status
		WellapadTaskCompletionStates status = WellapadMissionController.Instance.GetTaskStatus(task, bPop);
		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if(status == WellapadTaskCompletionStates.Completed ||
		   (status == WellapadTaskCompletionStates.RecentlyCompleted && bPop)){
			// mark this task as done
			checkTween.gameObject.SetActive(true);
			StartCoroutine(CheckboxSpriteShowHelper());	// Show after one frame
			rewardButton.SetActive(true);
		}
	}

	private IEnumerator CheckboxSpriteShowHelper(){
		yield return 0;
		checkTween.Show();
	}

	/// </summary>
	/// Callback for when the wellapad UI is done tweening.
	/// </summary>
	private void OnTweenDone(object sender, UIManagerEventArgs args){
		// if the UI is opening, update our task
		if(args.Opening){
			SetCheckboxSprite(true);
		}
	}
			
	void OnDestroy(){
		if(WellapadUIManager.Instance){
			WellapadUIManager.Instance.OnTweenDone -= OnTweenDone;
		}
	}

	public GameObject GetRewardButton(){
		return rewardButton;
	}

	public void HideRewardButton(){
		rewardButton.SetActive(false);
	}
}
