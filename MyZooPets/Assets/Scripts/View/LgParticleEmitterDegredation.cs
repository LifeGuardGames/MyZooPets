using UnityEngine;
using System.Collections;

public class LgParticleEmitterDegredation : LgParticleEmitter {
	
	public GameObject targetDestination;
	
	protected override void _ExtendedAction(GameObject emittedObject){
		MoveTowards moveScript = emittedObject.GetComponent<MoveTowards>();
		if(moveScript != null){
			moveScript.Target = targetDestination.transform;
		}
		else{
			Debug.LogError("No MoveTowards script detected in particle");
		}
	}
}
