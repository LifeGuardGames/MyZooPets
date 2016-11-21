using UnityEngine;
using System.Collections;

public class DustBagPowerUp : NinjaTrigger {

	protected override void _OnCut(){
		NinjaGameManager.Instance.bonusRound = true;
		NinjaGameManager.Instance.StartBonusVisuals();
		Destroy(this.gameObject);
	}

	void OnBecameVisible() {
		this.GetComponent<Rigidbody>().detectCollisions = true;
	}
}
