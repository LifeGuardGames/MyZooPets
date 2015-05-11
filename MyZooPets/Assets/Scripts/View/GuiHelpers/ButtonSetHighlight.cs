using UnityEngine;
using System;
using System.Collections;

public class ButtonSetHighlight : MonoBehaviour {
	
	public LgButton[] buttonList;	// List of locations that needs to move
	public LgButton firstButton;	// Starting location
	
	void Start(){
		if(firstButton != null){
			gameObject.transform.position = firstButton.transform.position;
		}
		else{
			GetComponent<UISprite>().enabled = false;
		}
		
		foreach(LgButton button in buttonList){
			button.OnProcessed += ChangePosition;
		}

		_Start();
	}

	// Override this in child, used for conditional position changes
	protected virtual void _Start(){}

	public void SetFirstButton(){
		if(firstButton != null){
			gameObject.transform.position = firstButton.transform.position;
		}
	}
	
	private void ChangePosition(object sender, EventArgs args){
		GetComponent<UISprite>().enabled = true;
		LgButton button = (LgButton)sender;
		gameObject.transform.position = button.transform.position;
	}
}
