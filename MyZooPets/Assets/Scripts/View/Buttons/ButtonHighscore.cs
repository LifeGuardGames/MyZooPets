using UnityEngine;
using System.Collections;

/// <summary>
/// Button that opens the HighscoreUI
/// </summary>
public class ButtonHighscore : LgButton {

	protected override void ProcessClick(){
		HighscoreUIManager.Instance.OpenUI();
	}
}
