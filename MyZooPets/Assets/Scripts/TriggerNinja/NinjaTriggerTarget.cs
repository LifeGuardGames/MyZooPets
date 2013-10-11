using UnityEngine;
using System.Collections;

//---------------------------------------------------
// NinjaTriggerTarget
// This is like a piece of fruit from Fruit Ninja;
// it is a positive object that the player wants to
// destroy.
//---------------------------------------------------	

public class NinjaTriggerTarget : NinjaTrigger {
	// how much is this trigger worth when the player cuts it?
	public int nPoints;
	public int GetPointValue() {
		return nPoints;	
	}
	
	//---------------------------------------------------
	// _OnCut()
	//---------------------------------------------------		
	protected override void _OnCut() {
		// award points
		int nVal = GetPointValue();
		NinjaManager.Instance.UpdateScore( nVal );		
	}
	
	//---------------------------------------------------
	// _OnMissed()
	//---------------------------------------------------	
	protected override void _OnMissed() {
		// the player loses a life
		NinjaManager.Instance.UpdateLives( -1 );	
	}	
}
