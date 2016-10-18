using UnityEngine;

public class ShooterUIManager : MonoBehaviour {

	// Controls for shooter
	public void OnMoveDown() {
		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
			ShooterGameManager.Instance.InputReceived(true);
		}
	}

	public void OnShootDown() {
		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
			ShooterGameManager.Instance.InputReceived(false);
		}
	}
}
