using UnityEngine.UI;
using UnityEngine;

public class RunnerGameUIManager : MonoBehaviour {
	public Text distanceLabel;
	public Text coinsLabel;
	public Text comboLabel;

	public TweenToggleDemux controlsDemux;      // Soft buttons to fade out once clicked
	public Animation jumpButtonAnim;
	public Animation dropButtonAnim;
	private bool isDemuxSet = false;

	public void UpdateUI(int distanceTraveled, int coins, float combo) {
		distanceLabel.text = distanceTraveled.ToString() + "m";
		coinsLabel.text = coins.ToString();
		comboLabel.text = "x" + combo.ToString("F1");
	}

	public void ResetControlsFade() {
		isDemuxSet = false;
		controlsDemux.Show();
	}

	public void OnJumpButton() {
		PlayerController.Instance.Jump();
		jumpButtonAnim.Play();
        if(!isDemuxSet && !RunnerGameManager.Instance.IsTutorialRunning) {
			controlsDemux.Hide();
			isDemuxSet = true;
		}
	}

	public void OnDropButton() {
		PlayerController.Instance.Drop();
		dropButtonAnim.Play();
		if(!isDemuxSet && !RunnerGameManager.Instance.IsTutorialRunning) {
			controlsDemux.Hide();
			isDemuxSet = true;
		}
	}
}
