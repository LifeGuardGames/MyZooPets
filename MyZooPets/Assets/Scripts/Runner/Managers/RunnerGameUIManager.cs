using UnityEngine.UI;
using UnityEngine;

public class RunnerGameUIManager : MonoBehaviour {
	public Text distanceLabel;
	public Text coinsLabel;
	public Text comboLabel;

	public void UpdateUI(int distanceTraveled, int coins, float combo) {
		distanceLabel.text = distanceTraveled.ToString() + " M";
		coinsLabel.text = coins.ToString();
		comboLabel.text = "x" + combo.ToString("F1");
	}
}
