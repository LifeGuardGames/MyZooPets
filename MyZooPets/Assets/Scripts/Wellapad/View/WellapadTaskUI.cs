using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//---------------------------------------------------
// WellapadTaskUI
// This is a UI that is just one task in a list
// of tasks for a given mission on the Wellapad mission
// list.
//---------------------------------------------------
public class WellapadTaskUI : MonoBehaviour {
	private MutableDataWellapadTask task;   // task belonging to this UI
	public Text label;                      // task text
	public TweenToggle slash;               // tween object for when the task is completed
	public Color tutTextHighlightOn;
	public Color tutTextHighlightOff;
	public GameObject RewardButton;

	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init(MutableDataWellapadTask task) {
		// cache the task
		this.task = task;

		// set the description for this task
		SetDesc();

		// set the checkbox sprite appropriately
		SetCheckboxSprite(false);

		// listen for various messages
		WellapadUIManager.Instance.OnTweenDone += OnTweenDone;                      // whent he ui finishes tweening
	}

	//---------------------------------------------------
	// SetDesc()
	// Sets the description for this task.
	//---------------------------------------------------	
	private void SetDesc() {
		// set the label showing what the task entails
		ImmutableDataWellapadTask data = DataLoaderWellapadTasks.GetTask(task.TaskID);

		string strDesc = data.GetText();
		// if the task has an amount, we want to integrate that into the string
		if(task.Amount > 0) {
			strDesc = string.Format(strDesc, task.Amount);
		}

		label.text = strDesc;
	}

	//---------------------------------------------------
	// SetCheckboxSprite()
	// Sets the sprite on this UI's checkbox based on
	// the status of the task.
	//---------------------------------------------------	
	private void SetCheckboxSprite(bool bPop) {
		// get the status
		WellapadTaskCompletionStates eStatus = WellapadMissionController.Instance.GetTaskStatus(task, bPop);

		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if(eStatus == WellapadTaskCompletionStates.Completed ||
			(eStatus == WellapadTaskCompletionStates.RecentlyCompleted && bPop)) {
			// mark this task as done
			slash.gameObject.SetActive(true);
			StartCoroutine(CheckboxSpriteShowHelper()); // Show after one frame
		}
	}

	private IEnumerator CheckboxSpriteShowHelper() {
		yield return 0;
		slash.Show();
	}

	//---------------------------------------------------
	// OnTweenDone()
	// Callback for when the wellapad UI is done tweening.
	//---------------------------------------------------		
	private void OnTweenDone(object sender, UIManagerEventArgs args) {
		// if the UI is opening, update our task
		if(args.Opening)
			SetCheckboxSprite(true);
	}

	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	void OnDestroy() {
		if(WellapadUIManager.Instance) {
			WellapadUIManager.Instance.OnTweenDone -= OnTweenDone;
		}
	}
}
