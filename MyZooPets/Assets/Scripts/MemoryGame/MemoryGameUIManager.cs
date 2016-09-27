using UnityEngine;
using UnityEngine.UI;

public class MemoryGameUIManager : MonoBehaviour{
	public Text labelComboScore;
	public Text score;
	public Image backBoard;
	public TweenToggleDemux memoryHudDemux;

	public void SetComboText(int comboScore){
		labelComboScore.text = comboScore.ToString();
	}

	public void UpdateScoreText(int _score) {
		score.text = _score.ToString();
	}

	public void StartBoard(){
		backBoard.gameObject.SetActive(true);
		memoryHudDemux.Show();
	}

	public void FinishBoard(){
		backBoard.gameObject.SetActive(false);
		memoryHudDemux.Hide();
	}
}
