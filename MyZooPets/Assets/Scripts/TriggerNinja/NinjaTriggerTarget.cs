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
		
		// increase the player's combo
		NinjaManager.Instance.IncreaseCombo( 1 );
		
		// then launch the trigger into the air
		
		// get proper forces
		int nForceRangeX = Constants.GetConstant<int>("CutForceRangeX");
		int nForceX = UnityEngine.Random.Range( -nForceRangeX, nForceRangeX );
		int nForceY = Constants.GetConstant<int>("CutForceY");
		Vector3 vForce = new Vector3( nForceX, nForceY, 0 );
		
		// apply said force
		gameObject.rigidbody.AddForce( vForce );
	}
	
	//---------------------------------------------------
	// _OnMissed()
	//---------------------------------------------------	
	protected override void _OnMissed() {
		// the player loses a life
		NinjaManager.Instance.UpdateLives( -1 );	
	}	
}
