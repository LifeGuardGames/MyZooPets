using UnityEngine;
using System.Collections;

public class DustBagPowerUp : NinjaTrigger {

	protected override void _OnCut(){
		NinjaManager.Instance.bonusRound = true;
		NinjaManager.Instance.StartBonusVisuals();
		Destroy(this.gameObject);
	}

	void OnBecameVisible() {
		this.GetComponent<Rigidbody>().detectCollisions = true;
	}
}
