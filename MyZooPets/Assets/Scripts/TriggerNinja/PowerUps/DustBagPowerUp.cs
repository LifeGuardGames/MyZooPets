using UnityEngine;
using System.Collections;

public class DustBagPowerUp : NinjaTrigger {

	protected override void _OnCut(){
		NinjaManager.Instance.bonusRound = true;
		Destroy(this.gameObject);
	}
}
