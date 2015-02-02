using UnityEngine;
using System.Collections;

public class DustBagPUp : NinjaTrigger {

	protected override void _OnCut(){
		NinjaManager.Instance.bonusRound = true;
		Destroy(this.gameObject);
	}
}
