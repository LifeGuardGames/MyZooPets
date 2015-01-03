using UnityEngine;
using System.Collections;
using System;

public class MemoryGameUIManager : MinigameUI{

	public UILabel labelComboScore;

	public void SetComboText(int comboScore){
		labelComboScore.text = comboScore.ToString();
	}
}
