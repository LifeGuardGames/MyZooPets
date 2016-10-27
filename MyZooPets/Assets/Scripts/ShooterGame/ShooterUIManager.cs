using UnityEngine;

public class ShooterUIManager : MonoBehaviour {

	// Controls for shooter
	public void OnMoveDown() {
		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
			ShooterGameManager.Instance.InputReceivedMove(true);
		}
	}

	public void OnShootDown() {

		if(!ShooterGameManager.Instance.isGameOver && !ShooterGameManager.Instance.IsPaused) {
#if !UNITY_EDITOR
			Touch pos;
			if(Input.touchCount > 1){
				 pos = Input.GetTouch(1);
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
