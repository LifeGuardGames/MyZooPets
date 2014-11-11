using UnityEngine;
using System.Collections;
using System;

public class MemoryGameUIManager : MinigameUI {
	public UILabel timeLeftLabel;

	public void DisplayTimeLeft(float timeLeft){
		timeLeftLabel.text = ((int)Math.Ceiling(timeLeft)).ToString();
	}
}
