using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// WellapadTaskUI
// This is a UI that is just one task in a list
// of tasks for a given mission on the Wellapad mission
// list.
//---------------------------------------------------

public class WellapadTaskUI : MonoBehaviour {	
	// task belonging to this UI
	private MutableDataWellapadTask task;
	
	// task text
	public UILabel label;
	
	// tween object for when the task is completed
	public TweenToggle slash;
	public TweenToggle tweenCheck;
	
	public Color tutTextHighlightOn;
	public Color tutTextHighlightOff;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( MutableDataWellapadTask task ) {
		// cache the task
		this.task = task;
		
		// set the description for this task
		SetDesc();
		
		// set the checkbox sprite appropriately
		SetCheckboxSprite( false );
		
		// listen for various messages
		WellapadMissionController.Instance.OnHighlightTask += OnTaskHighlighted;	// when a task may be highlighted
		WellapadUIManager.Instance.OnTweenDone += OnTweenDone;						// whent he ui finishes tweening
	}
	
	//---------------------------------------------------
	// SetDesc()
	// Sets the description for this task.
	//---------------------------------------------------	
	private void SetDesc() {
		// set the label showing what the task entails
		ImmutableDataWellapadTask data = DataLoaderWellapadTasks.GetTask( task.TaskID );
		string strDesc = data.GetText();
		
		// if the task has an amount, we want to integrate that into the string
		if ( task.Amount > 0 )
			strDesc = String.Format(strDesc, task.Amount);	

		label.text = strDesc;			
	}
	
	//---------------------------------------------------
	// SetCheckboxSprite()
	// Sets the sprite on this UI's checkbox based on
	// the status of the task.
	//---------------------------------------------------	
	private void SetCheckboxSprite( bool bPop ) {
		// get the status
		WellapadTaskCompletionStates eStatus = WellapadMissionController.Instance.GetTaskStatus( task, bPop );
		
		// show the tween only if the status is complete OR the status is recently completed and we are popping the task status
		if ( eStatus == WellapadTaskCompletionStates.Completed ||
				( eStatus == WellapadTaskCompletionStates.RecentlyCompleted && bPop ) ) {
			// mark this task as done
			tweenCheck.Show();
			slash.gameObject.SetActive(true);
			slash.Show();
		}		
	}
	
	//---------------------------------------------------
	// OnTweenDone()
	// Callback for when the wellapad UI is done tweening.
	//---------------------------------------------------		
	private void OnTweenDone( object sender, UIManagerEventArgs args ) {
		// if the UI is opening, update our task
		if ( args.Opening )
			SetCheckboxSprite( true );
	}	
	
	//---------------------------------------------------
	// OnTaskHighlighted()
	// Callback for when a task is highlighted
	//---------------------------------------------------	
	private void OnTaskHighlighted( object sender, TaskUpdatedArgs args ) {
		if ( args.ID == task.TaskName ) {
			// this task is being highlighted -- change the text to black
			label.color = tutTextHighlightOn;
		}
		else {
			// this task is not being highlighted, so grey it out
			label.color = tutTextHighlightOff;
		}
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	void OnDestroy() {
		// stop listening for task completion data
		if ( WellapadMissionController.Instance )
			WellapadMissionController.Instance.OnHighlightTask -= OnTaskHighlighted;
		
		if ( WellapadUIManager.Instance )
			WellapadUIManager.Instance.OnTweenDone -= OnTweenDone;		
	}
}
