﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// WellapadTaskUI
// This is a UI that is just one task in a list
// of tasks for a given mission on the Wellapad mission
// list.
//---------------------------------------------------

public class WellapadTaskUI : MonoBehaviour {	
	// task belonging to this UI
	private Data_WellapadTask task;
	
	// task text
	public UILabel label;
	
	// checkbox sprite
	public UISprite spriteBox;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( Data_WellapadTask task ) {
		// cache the task
		this.task = task;
		
		// set the label showing what the task entails
		string strTask = task.GetText();
		label.text = strTask;	
		
		// set the checkbox sprite appropriately
		SetCheckboxSprite();
		
		// listen for various messages
		WellapadMissionController.Instance.OnTaskUpdated += OnTaskUpdated;			// when a task is complete so the UI can react
		WellapadMissionController.Instance.OnHighlightTask += OnTaskHighlighted;	// when a task may be highlighted
		
		
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
		if ( args.ID == task.GetID() )
			SetCheckboxSprite();
	}
	
	//---------------------------------------------------
	// OnTaskHighlighted()
	// Callback for when a task is highlighted
	//---------------------------------------------------	
	private void OnTaskHighlighted( object sender, TaskUpdatedArgs args ) {
		if ( args.ID == task.GetID() ) {
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