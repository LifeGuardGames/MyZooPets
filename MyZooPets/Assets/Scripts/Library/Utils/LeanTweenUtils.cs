using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LeanTweenUtils
// For any kind of utility function for lean tween.
//---------------------------------------------------

public static class LeanTweenUtils  {
	
	//---------------------------------------------------
	// MoveAlongPathWithSpeed()
	// Moves the incoming game object along a path with
	// a speed instead of time.  Distance is approximated
	// by finding the distance between all points along
	// the path.
	//---------------------------------------------------	
	public static void MoveAlongPathWithSpeed( GameObject goTarget, Vector3[] vPath, float fSpeed, Hashtable hashOptional ) {
		// calculate the total distance by finding the distance between each point in the path
		float fDistance = 0;
		for ( int i = 0; i < vPath.Length-1; ++i ) {
			Vector3 v1 = vPath[i];
			Vector3 v2 = vPath[i+1];
			fDistance += Vector3.Distance(v1, v2);
		}
		
		// the time the lean tween should take to complete
		float fTime = fDistance / fSpeed;
		
		// call lean tween to do the move
		LeanTween.move(goTarget, vPath, fTime, hashOptional);
		
		//Debug.Log("Beginning lean tween with time: " + fTime);
		//Debug.Log("Total distance: " + fDistance + " and time: " + fTime);
	}
}
