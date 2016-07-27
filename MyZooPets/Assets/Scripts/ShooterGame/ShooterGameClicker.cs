using UnityEngine;

public class ShooterGameClicker : MonoBehaviour {
	void OnTap(TapGesture e){
		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
			ShooterGameManager.Instance.OnTapped(e);
		}
	}
}