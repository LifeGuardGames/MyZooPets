using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////
// TrackObject
// Simple script that adds an object to a list when
// it is created, and removes it when it is destroyed.
/////////////////////////////////////////////////// 

public class TrackObject : MonoBehaviour {
	// list that keeps track of the objects
	private List<GameObject> listObjects;
	
    ///////////////////////////////////////////////////
    // Init() 
    /////////////////////////////////////////////////// 	
	public void Init( List<GameObject> list ) {
		// save the list
		listObjects = list;
		
		// add this object to the list
		if ( listObjects != null )
			listObjects.Add( gameObject );
	}
	
    ///////////////////////////////////////////////////
    // OnDestroy() 
    /////////////////////////////////////////////////// 	
	void OnDestroy() {
		// since the object is being destroyed, remove it from the list
		if ( listObjects != null )
			listObjects.Remove( gameObject );
	}
}
