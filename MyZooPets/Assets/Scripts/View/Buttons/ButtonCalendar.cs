using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonCalender
// Button that loads the calendar UI.
//---------------------------------------------------

public class ButtonCalendar : LgButton {
	
	protected override void ProcessClick() {
		CalendarUIManager.Instance.OpenUI();
	}
}
