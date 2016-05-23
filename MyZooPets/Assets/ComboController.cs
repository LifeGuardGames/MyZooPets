using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboController : MonoBehaviour {
	public Text scoreText;
	public Text comboText;

	public void UpdateScore(int newScore) {
		scoreText.text = "Score: \n" + newScore.ToString();
	}

	public void UpdateCombo(int newCombo) {
		comboText.text = "Combo: \n" + newCombo.ToString();
	}
}
