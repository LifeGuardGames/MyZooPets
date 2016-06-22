using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class MemoryGameUIManager : MonoBehaviour{

	public Text labelComboScore;
	public Text score;
	public Image backboard;
	public TweenToggleDemux memoryHudDemux;

	public void SetComboText(int comboScore){
		labelComboScore.text = comboScore.ToString();
	}

	public void UpdateScoreText(int _score) {
		score.text = _score.ToString();
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
