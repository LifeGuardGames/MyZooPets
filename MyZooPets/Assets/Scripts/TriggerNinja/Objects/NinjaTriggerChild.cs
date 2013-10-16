using UnityEngine;
using System.Collections;

//---------------------------------------------------
// NinjaTriggerChild
// Some triggers are actually just empty parents with
// children that are the real models.  This script
// should be on all such children so they can alert
// the parent when they become invisible.
//---------------------------------------------------	

public class NinjaTriggerChild : MonoBehaviour {	
	//---------------------------------------------------
	// OnBecameInvisible()
	// Nifty callback function that will tell us when
	// the object is no longer being rendered by the
	// camera.
	//---------------------------------------------------		
	void OnBecameInvisible() {
		NinjaTrigger triggerParent = transform.parent.gameObject.GetComponent<NinjaTrigger>();
		if ( triggerParent )
			triggerParent.ChildBecameInvis();
		else
			Debug.Log("Trigger child does not have a parent...");
	}
}
