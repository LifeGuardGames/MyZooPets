using UnityEngine;
using System.Collections;

public class MoveTowards : MonoBehaviour {

	public Transform target;
	public Transform Target{ 
		get{return target;}
		set{target = value;}
	}
	
	public float speed = 5.0f;
	public bool isDestroyOnTouch;
	public float destroyDelay = 0f;	// Delay before destroyed, useful for waiting on child particle effects to finish
	
	public GameObject touchCallbackTarget;
	public string touchCallbackFunctionName;
	
	private bool finished = false;	// Aux bool used to limit callback to one frame
	
	void Update(){
		if(target != null){
			// Move towards destination gameobject at 'speed' amount every frame
		    transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
			
			// Suicide when touched?
			if(isDestroyOnTouch){
				if(transform.position == target.transform.position && finished == false){
					finished = true;
					if(gameObject.renderer != null){
						gameObject.renderer.enabled = false;
					}
					DoCallback();
					Destroy(gameObject, destroyDelay);
				}
			}
		}
	}
	
    ///////////////////////////////////////////////////
    // FlashToTarget()
	// Moves this object instantly to the target.
    ///////////////////////////////////////////////////	
	public void FlashToTarget() {
		transform.position = target.position;	
	}
	
	// Code executed when the gameobject touches target
	private void DoCallback(){
		
		// Check to see if we need to call anything
		if(touchCallbackFunctionName != null){
			
			// Assign it to self if it is empty
			if(touchCallbackTarget == null){
				touchCallbackTarget = gameObject;
			}
			
			// Send the message with self reference
			touchCallbackTarget.BroadcastMessage(touchCallbackFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
