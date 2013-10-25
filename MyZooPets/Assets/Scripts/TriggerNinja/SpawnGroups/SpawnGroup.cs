﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// SpawnGroup
// Abstract class used to define how a group of
// objects in the trigger ninja game is spawned.
//---------------------------------------------------

public abstract class SpawnGroup {
	// constants
	protected const float SPAWN_Y = -.25f;	// where on the y-axis objects spawn
	protected const float SPAWN_Z = 10f;	// where on the z-axis objects spawn
	
	// list of valid x-locations to spawn triggers
	protected static List<float> listLocations;
	
	//---------------------------------------------------
	// SpawnGroup()
	//---------------------------------------------------
	public SpawnGroup( List<string> listObjects ) {
		// if the static list of locations hasn't been created yet, int it
		if ( listLocations == null )
			InitLocations();
		
		// check the number of objects to be spawned
		if ( !CheckCount( listObjects ) ) {
			Debug.Log("Incorrect number of objects to be spawned for " + this);
			return;
		}
		
		// if the count is good, let's spawn the objects
		SpawnObjects( listObjects );
	}
	
	//---------------------------------------------------
	// InitLocations()
	// Create and store a list of valid spawning locations
	// along the x-axis.
	//---------------------------------------------------	
	private static void InitLocations() {
		listLocations = new List<float>();
		for ( float i = .1f; i <= 1f; i += .1f ) 
			listLocations.Add( i );
	}
	
	//---------------------------------------------------
	// GetRandomForce()
	// Returns a random force value to be applied to
	// an object.
	//---------------------------------------------------	
	protected static int GetRandomForce() {
		int nMin = Constants.GetConstant<int>("MinForceY");
		int nMax = Constants.GetConstant<int>("MaxForceY");
		int nForce = UnityEngine.Random.Range(nMin, nMax);
		
		return nForce;
	}
	
	//---------------------------------------------------
	// CheckCount()
	// Children classes implement this function to do some
	// checking to make sure that the number of objects
	// to spawn fits in with the paradigm of that group.
	//---------------------------------------------------	
	protected abstract bool CheckCount( List<string> listObjects );
	
	//---------------------------------------------------
	// SpawnObjects()
	// Spawns a number of objects according to the child's
	// spawning paradigm.
	//---------------------------------------------------		
	protected abstract void SpawnObjects( List<string> listObjects );
	
	//---------------------------------------------------
	// SpawnObjects()
	// Spawns a list of objects at a given location.
	//---------------------------------------------------	
	protected virtual void SpawnObjects( List<string> listObjects, List<float> listSpawnLocs ) {
		// just in case
		if ( listObjects.Count != listSpawnLocs.Count ) {
			Debug.Log("Spawn location and count don't match");
			return;
		}
		
		for ( int i = 0; i < listObjects.Count; ++i ) {
			// get the info about object to be spawned
			string strResource = listObjects[i];
			float fLoc = listSpawnLocs[i];
			
			// spawn the object
			SpawnObject( strResource, fLoc );
		}			
	}
	
	//---------------------------------------------------
	// SpawnObject()
	// Spawns an object at a given location.
	//---------------------------------------------------	
	protected virtual void SpawnObject( string strObjectResource, float fX ) {
		// create force for the object
		float fForceY = GetRandomForce();
		Vector3 vForce = new Vector3( 0, fForceY, 0 );
		
		// pass it along
		SpawnObject( strObjectResource, fX, vForce );	
	}
	
	//---------------------------------------------------
	// SpawnObject()
	// Override with force specified.
	//---------------------------------------------------		
	protected void SpawnObject( string strObjectResource, float fX, Vector3 vForce ) {
		// spawn the object at the proper location
		GameObject resource = Resources.Load(strObjectResource) as GameObject;
		Vector3 vPos = Camera.main.ViewportToWorldPoint( new Vector3( fX, SPAWN_Y, SPAWN_Z ) );
		GameObject go = GameObject.Instantiate( resource, vPos, resource.transform.rotation ) as GameObject;
		
		// add force to the object
		go.rigidbody.AddForce( vForce );	
		
		// add some torque to the object as well
		float fTorqueRange = Constants.GetConstant<float>("TorqueRange");
		float fTorqueX = UnityEngine.Random.Range(-fTorqueRange, fTorqueRange);
		float fTorqueY = UnityEngine.Random.Range(-fTorqueRange, fTorqueRange);
		float fTorqueZ = UnityEngine.Random.Range(-fTorqueRange, fTorqueRange);
		go.rigidbody.AddTorque( new Vector3( fTorqueX, fTorqueY, fTorqueZ ) );
	}
}