using UnityEngine;
using System.Collections;

public class ShooterGameClicker : MonoBehaviour {
	void OnTap(TapGesture e){
		ShooterGameManager.Instance.OnTapped(e);
	}
}