using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboController : MonoBehaviour{
	public Text scoreText;
	public Text comboText;
	//Scrolling table
	private int lastCombo = 0;
	private int combo = 0;
	//Maximum amount of time for a correct diagnosis
	private float timeToCombo = 3f;
	private float currentComboTime = 0;
	private int comboBonus = 5;
	private bool countingDown = true;

	void Update(){
		if(DoctorMatchGameManager.Instance.Paused || !countingDown)
			return;
		if(currentComboTime > 0){
			currentComboTime -= Time.deltaTime;
		}
		else if(combo != 0){
			ResetCombo();
		}
	}

	public int Combo{
		get{ return combo; }
	}

	public int ComboMod{
		get { return Mathf.Clamp(combo, 0, (comboBonus * 2) - 1); }
	}

	public int ComboLevel{
		get{
			if((combo + 1) % (comboBonus * 2) == 0 && combo != 0){ //Big combo bonus
				return 2;
			}
			else if((combo + 1) % comboBonus == 0 && combo != 0){ //Small combo bonus
				return 1;
			}
			else{
				return 0;
			}
		}
	}

	public void StartCounting(){
		countingDown = true;
	}

	public void StopCounting(){
		countingDown = false;
	}

	public void ResetCombo(){
		combo = 0;
		currentComboTime = 0;
		UpdateCombo();
	}

	public void Setup(){
		GameObject slotObject;
	}

	public void UpdateScore(int newScore){
		scoreText.text = "Score: " + newScore.ToString();
	}

	public void IncrementCombo(){
		combo++;
		UpdateCombo();
		currentComboTime = timeToCombo;
	}

	private void UpdateCombo(){
		comboText.text = "x" + combo.ToString();
		lastCombo = ComboMod;
	}
}
