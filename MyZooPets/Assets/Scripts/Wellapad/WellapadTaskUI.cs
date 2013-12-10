using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadTaskUI
// This is a UI that is just one task in a list
// of tasks for a given mission on the Wellapad mission
// list.
//---------------------------------------------------

public class WellapadTaskUI : MonoBehaviour {	
	// task belonging to this UI
	private WellapadTask task;
	
	// task text
	public UILabel label;
	
	// checkbox sprite
	public UISprite spriteBox;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( WellapadTask task ) {
		// cache the task
		this.task = task;
		
		// set the description for this task
		SetDesc();
		
		// set the checkbox sprite appropriately
		SetCheckboxSprite();
		
		// listen for various messages
		WellapadMissionController.Instance.OnTaskUpdated += OnTaskUpdated;			// when a task is complete so the UI can react
		WellapadMissionController.Instance.OnHighlightTask += OnTaskHighlighted;	// when a task may be highlighted
	}
	
	//---------------------------------------------------
	// SetDesc()
	// Sets the description for this task.
	//---------------------------------------------------	
	private void SetDesc() {
		// set the label showing what the task entails
		string strTask = task.GetDesc();
		label.text = strTask;			
	}
	
	//---------------------------------------------------
	// SetCheckboxSprite()
	// Sets the sprite on this UI's checkbox based on
	// the status of the task.
	//---------------------------------------------------	
	private void SetCheckboxSprite() {
		// get the status
		bool bStatus = WellapadMissionController.Instance.GetTaskStatus( task );
		
		if ( bStatus ) {
			// mark this task as done
			string strSprite = Constants.GetConstant<string>( "CheckedSprite" );
			spriteBox.spriteName = strSprite;
		}		
	}
	
	//---------------------------------------------------
	// OnTaskUpdated()
	// Callback for when a task's status gets updated.
	//---------------------------------------------------		
	private void OnTaskUpdated( object sender, TaskUpdatedArgs args ) {
		// if the IDs match, update our checkbox sprite
		if ( args.ID == task.TaskID )
			SetCheckboxSprite();
	}
	
	//---------------------------------------------------
	// OnTaskHighlighted()
	// Callback for when a task is highlighted
	//---------------------------------------------------	
	private void OnTaskHighlighted( object sender, TaskUpdatedArgs args ) {
		if ( args.ID == task.TaskID ) {
			// this task is being highlighted -- change the text to black
			label.color = Color.black;
		}
		else {
			// this task is not being highlighted, so grey it out
			label.color = Color.gray;
		}
	}
	
	//---------------------------------------------------
	// OnDestory()
	//---------------------------------------------------		
	void OnDestroy() {
		// stop listening for task completion data
		if ( WellapadMissionController.Instance )
			WellapadMissionController.Instance.OnTaskUpdated -= OnTaskUpdated;
	}
}
