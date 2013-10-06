using UnityEngine;
using System.Collections;

/// <summary>
/// Tutorial popup controller.
/// Wrapper interface for tutorial popup prefabs. This controls turning on/off buttons, show/hide,
/// and other populating things.
/// </summary>

public class PopupNotificationTutorial: PopupNotificationNGUI {
	public GameObject contentSprite;
	public GameObject buttonHintArrow;
	public UILabel buttonLabel;
	public GameObject calendarTutorialMessage;

	void Awake(){
		buttonHintArrow.SetActive(false);
	}

	public void SetButtonText(string text){
		buttonLabel.text = text;
	}

	//Set the image that needs to be displayed
	public void SetContent(TutorialImageType imageType){
		string spriteName = "";
		switch(imageType){
			case TutorialImageType.CalendarIntro:
				spriteName = "tutorialCalendar0"; 
				calendarTutorialMessage.SetActive(true);
				calendarTutorialMessage.transform.Find("Label_PetName").GetComponent<UILabel>().
					text = DataManager.Instance.GameData.PetInfo.PetName;
			break;
			case TutorialImageType.CalendarGreenStamp: spriteName = "tutorialCalendar1"; break;
			case TutorialImageType.CalendarRedStamp: spriteName = "tutorialCalendar2"; break;
			case TutorialImageType.CalendarBonus: spriteName = "tutorialCalendar3"; break;
		}	
		UISprite sprite = contentSprite.GetComponent<UISprite>();
		sprite.spriteName = spriteName;
	}
}
