using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NinjaUIManager : MonoBehaviour {
	public Camera UICamera;
	public List<Image> InhalerLifeList;
	public GameObject FloatyParent;
	public GameObject FloatyComboPrefab;
	public Text TimerText;
	public Image Timer;

	public void NewGameUI() {
		// Set all life to true
		foreach(Image inhalerSprite in InhalerLifeList) {
			ToggleLifeIndicator(true, inhalerSprite);
		}
	}
	public void ContinueGameUI() {
		// Set all life to true
		ToggleLifeIndicator(true, InhalerLifeList[0]);
		
	}

	#region Minigame Life UI
	public void OnLivesChanged(int deltaLife) {
		if(LoadLevelManager.Instance.GetCurrentSceneName() == "TriggerNinja") {
			// get the number of lives there are
			int lifeCount = NinjaGameManager.Instance.LifeCount;
			int nChange = deltaLife;
			if(nChange < 0 && lifeCount + nChange >= -1) {
				// if we are LOSING a life and the current lives +1 == this life's index, it means that this life was just lost, so toggle off
				ToggleLifeIndicator(false, InhalerLifeList[lifeCount]);

				// Play the camera shake animation
				if(Camera.main.GetComponent<Animation>() != null) {
					Camera.main.GetComponent<Animation>().Play();
					if(lifeCount == 0) {
						NinjaGameManager.Instance.GameOver();
					}
				}
			}
			else if(nChange > 0 && lifeCount - nChange >= 0) {
				//Debug.Log("---Gaining a life");
				// else if we are GAINING a life and the current lives == this life's index, it means this life was just gained, so toggl eon
				ToggleLifeIndicator(true, InhalerLifeList[lifeCount]);
			}
		}
	}

	public void ToggleLifeIndicator(bool isOn, Image inhalerSprite) {
		// change the tint based on on/off
		inhalerSprite.color = isOn ? Color.white : Color.black;
	}
	#endregion

	public void SpawnComboFloaty(Vector3 position, int combo) {
		string comboText = string.Format(Localization.Localize("NINJA_COMBO"), combo);
		GameObject floatyObject = GameObjectUtils.AddChildGUI(FloatyParent, FloatyComboPrefab);
        floatyObject.GetComponent<FloatyController>().InitAndActivate(position, customText:comboText);
	}

	public void ShowTimer() {
		Timer.gameObject.SetActive(true);
	}
	public void UpdateTimer(float time) {
		string tempTime = time.ToString();
		TimerText.text = "Time: " + tempTime.Substring(0,2);
	}
}
