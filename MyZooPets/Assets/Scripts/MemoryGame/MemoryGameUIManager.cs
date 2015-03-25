using UnityEngine;
using System.Collections;
using System;

public class MemoryGameUIManager : MinigameUI{

	public UILabel labelComboScore;
	public UISprite backboard;
	public TweenToggleDemux memoryHudDemux;

	public void SetComboText(int comboScore){
		labelComboScore.text = comboScore.ToString();
	}

	public void StartBoard(){
		backboard.gameObject.SetActive(true);
		memoryHudDemux.Show();
	}

	public void FinishBoard(){
		backboard.gameObject.SetActive(false);
		memoryHudDemux.Hide();
	}
}
