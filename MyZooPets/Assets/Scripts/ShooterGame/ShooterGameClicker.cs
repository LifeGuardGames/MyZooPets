using UnityEngine;

public class ShooterGameClicker : MonoBehaviour {
	void OnTap(TapGesture e){
		ShooterGameManager.Instance.OnTapped(e);
	}
}