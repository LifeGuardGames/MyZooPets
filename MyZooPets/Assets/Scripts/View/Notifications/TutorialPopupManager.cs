using UnityEngine;
using System.Collections;

/// <summary>
/// Tutorial popup controller.
/// Wrapper interface for tutorial popup prefabs. This controls turning on/off buttons, show/hide,
/// and other populating things.
/// </summary>

public class TutorialPopupManager : BackDrop {

	public GameObject titleSprite;
	public GameObject button1;
	public GameObject contentSprite;

	protected override void Awake(){
		base.Awake();
		backDropParent = gameObject;
	}

	//Handler for button
	public void OnButtonClick(){
		Hide();
	}

	public void SetButtonMessage(string message){
		// button1.transform.Find()
	}

	//Function to be called after the tutorial panel hides itself
	public void SetButtonCallBack(GameObject target, string functionName){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		moveToggleDemux.isHideFinishedCallback = true;
		moveToggleDemux.HideTarget = target;
		moveToggleDemux.HideFunctionName = functionName;
	}

	//Set the image that needs to be displayed
	public void SetContent(string spriteName){
		UISprite sprite = contentSprite.GetComponent<UISprite>();
		sprite.spriteName = spriteName;
	}

	public void Display(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		if(moveToggleDemux != null){
			moveToggleDemux.Show();
			DisplayBackDrop();
		}else{
			Debug.LogError("No move tween script detected");
		}
	}

	private void Hide(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		if(moveToggleDemux != null){
			moveToggleDemux.Hide();
			HideBackDrop();
			Destroy(gameObject, 1f);
		}else{
			Debug.LogError("No move tween script detected");
		}
	}
}
