using UnityEngine;
using UnityEngine.UI;

public class ShooterUIManager : MonoBehaviour {

	public GameObject Timer;
	public Text timerText;
	// Controls for shooter
	public void OnMoveDown() {
		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
			ShooterGameManager.Instance.InputReceivedMove(true);
		}
	}

	public void ShowTimer() {
		Timer.gameObject.SetActive(true);
	}
	public void UpdateTimer(float time) {
		string tempTime = time.ToString();
		timerText.text = "Time: " +  tempTime.Substring(0,2);
	}

	public void OnShootDown() {
		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
#if !UNITY_EDITOR
			Touch pos;
			if(Input.touchCount > 1){
				if(Input.GetTouch(1).position.x > 187f){
					 pos = Input.GetTouch(1);
				}
				else{
					pos = Input.GetTouch(0);
				}
			}
			else{
				 pos = Input.GetTouch(0);
			}
			ShooterGameManager.Instance.InputReceivedShoot(false, pos.position);
#endif
			Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
			ShooterGameManager.Instance.InputReceivedShoot(false,mousePos);
		}
	}
}
