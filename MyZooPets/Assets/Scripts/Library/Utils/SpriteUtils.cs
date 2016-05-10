using UnityEngine;
using System.Collections;

//---------------------------------------------------
// SpriteUtils
// For any kind of utility function for our 2d sprites.
//---------------------------------------------------

public static class SpriteUtils  {
	
	//---------------------------------------------------
	// PointTo()
	// Points goFrom to goTo over fTime seconds.
	//---------------------------------------------------	
	public static void PointTo( GameObject goFrom, GameObject goTo, float fTime ) {
		Vector3 vFrom = goFrom.transform.position;
		Vector3 vTo = goTo.transform.position;
		float fAngle = Mathf.Rad2Deg * Mathf.Atan2( vFrom.y - vTo.y, vFrom.x - vTo.x );
		
		LeanTween.rotateZ(goFrom, fAngle, fTime);		
	}
}
