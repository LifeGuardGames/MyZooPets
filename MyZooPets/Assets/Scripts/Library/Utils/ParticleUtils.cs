using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ParticleUtils
// For any kind of utility functions for particles.
//---------------------------------------------------

public static class ParticleUtils {
	
	//---------------------------------------------------
	// CreateParticle()
	// Creates a shuriken particle effect at the incoming
	// location.
	//---------------------------------------------------	
	public static GameObject CreateParticle( GameObject goResource, Vector3 vPos ) {
		GameObject goParticle = GameObject.Instantiate( goResource, vPos, Quaternion.identity ) as GameObject;
		
		return goParticle;
	}
	public static GameObject CreateParticle( string strResource, Vector3 vPos ) {
		GameObject goResource = Resources.Load( strResource ) as GameObject;
		GameObject goParticle = CreateParticle( goResource, vPos );
		
		return goParticle;
	}
}
