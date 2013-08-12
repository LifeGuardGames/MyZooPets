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
	public GameObject buttonHintArrow;
	public UILabel buttonLabel;
	public GameObject calendarTutorialMessage;

	protected override void Awake(){
		base.Awake();
		backDropParent = gameObject;
		buttonHintArrow.active = false;
	}

	//Handler for button
	public void OnButtonClick(){
		Hide();
	}

	public void SetButtonMessage(string message){
		buttonLabel.text = message;
	}

	//Function to be called after the tutorial panel hides itself
	public void SetButtonCallBack(GameObject target, string functionName){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		moveToggleDemux.isHideFinishedCallback = true;
		moveToggleDemux.HideTarget = target;
		moveToggleDemux.HideFunctionName = functionName;
	}

	//Set the image that needs to be displayed
	public void SetContent(TutorialImageType imageType){
		string spriteName = "";
		switch(imageType){
			case TutorialImageType.CalendarIntro: 
				spriteName = "tutorialCalendar0"; 
				calendarTutorialMessage.active = true;
				calendarTutorialMessage.transform.Find("Label_PetName").GetComponent<UILabel>().
					text = DataManager.Instance.PetName;
			break;
			case TutorialImageType.CalendarGreenStamp: spriteName = "tutorialCalendar1"; break;
			case TutorialImageType.CalendarRedStamp: spriteName = "tutorialCalendar2"; break;
			case TutorialImageType.CalendarBonus: spriteName = "tutorialCalendar3"; break;
		}	
		UISprite sprite = contentSprite.GetComponent<UISprite>();
		sprite.spriteName = spriteName;
	}

	public void Display(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		if(D.Assert(moveToggleDemux != null, "No move tween script detected")){
			moveToggleDemux.Show();
			DisplayBackDrop();
		}
	}

	private void Hide(){
		MoveTweenToggleDemultiplexer moveToggleDemux = this.GetComponent<MoveTweenToggleDemultiplexer>();
		if(D.Assert(moveToggleDemux != null, "No move tween script detect")){
			moveToggleDemux.Hide();
			HideBackDrop();
			Destroy(gameObject, 1f);
		}
	}
}
