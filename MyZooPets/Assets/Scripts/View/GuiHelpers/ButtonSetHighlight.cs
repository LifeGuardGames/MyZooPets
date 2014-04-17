using UnityEngine;
using System;
using System.Collections;

public class ButtonSetHighlight : MonoBehaviour {
	
	public LgButton[] buttonList;	// List of locations that needs to move
	public LgButton firstButton;	// Starting location
	
	void Start(){
		gameObject.transform.position = firstButton.transform.position;
		
		foreach(LgButton button in buttonList){
			button.OnProcessed += ChangePosition;
		}

		_Start();
	}

	// Override this in child, used for conditional position changes
	protected virtual void _Start(){}

	public void SetFirstButton(){
		gameObject.transform.position = firstButton.transform.position;
	}
	
	private void ChangePosition(object sender, EventArgs args){
		LgButton button = (LgButton)sender;
		gameObject.transform.position = button.transform.position;
	}
}
