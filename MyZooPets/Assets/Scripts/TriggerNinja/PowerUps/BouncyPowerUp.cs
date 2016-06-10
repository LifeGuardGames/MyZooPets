﻿using UnityEngine;
using System.Collections;

public class BouncyPowerUp : NinjaTrigger {

	protected override void _OnCut(){
		NinjaManager.Instance.BeginBoucePowerUp();
		Destroy(this.gameObject);
	}

	void OnBecameVisible() {
		this.GetComponent<Rigidbody>().detectCollisions = true;
	}
}
