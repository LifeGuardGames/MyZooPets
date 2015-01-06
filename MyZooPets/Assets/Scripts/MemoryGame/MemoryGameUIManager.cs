using UnityEngine;
using System.Collections;
using System;

public class MemoryGameUIManager : MinigameUI{

	public UILabel labelComboScore;
	public UISprite backboard;

	public void SetComboText(int comboScore){
		labelComboScore.text = comboScore.ToString();
//		FloatyUtil.SpawnFloatyText();
	}

	public void StartBoard(){
		backboard.gameObject.SetActive(true);
	}

	public void FinishBoard(){
		backboard.gameObject.SetActive(false);
	}
}
