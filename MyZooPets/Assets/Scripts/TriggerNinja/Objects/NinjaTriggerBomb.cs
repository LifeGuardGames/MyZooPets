﻿using UnityEngine;
using System.Collections;

public class NinjaTriggerBomb : NinjaTrigger {
	// how much damage does this trigger do when cut?
	public int nDamage;
	public int GetDamageValue() {
		return nDamage;	
	}
	
	//---------------------------------------------------
	// _OnCut()
	//---------------------------------------------------		
	protected override void _OnCut() {
		// take lives from the player
		int nLives = GetDamageValue();
		NinjaManager.Instance.UpdateLives( -nLives );
		
		// then destroy the object
		Destroy( gameObject );		
	}
}