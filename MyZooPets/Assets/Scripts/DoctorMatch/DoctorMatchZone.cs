using UnityEngine;
using System.Collections;

public class DoctorMatchZone : MonoBehaviour {

	public string zoneKey;	// Key to match with the item key
	public Vector3 hoverScale;
	private Collider2D auxItemCheck;
	private string auxItemCheckKey;
	private bool activeState;


	void OnTriggerEnter2D(Collider2D collider){
		SetActiveState(true);
		auxItemCheck = collider;
		auxItemCheckKey = collider.gameObject.GetComponent<AssemblyLineItem>().itemKey;
	}

	void OnTriggerExit2D(Collider2D collider){
		SetActiveState(false);
	}

	private void SetActiveState(bool state){
		activeState = state;
		if(state){
			gameObject.transform.localScale = hoverScale;
		}
		else{
			gameObject.transform.localScale = Vector3.one;
		}
	}
	
	void Update(){
		// Thing inside the zone was destroyed, so deactivate
		// user let go inside the zone
		if(auxItemCheck == null && activeState){
			SetActiveState(false);
		}
	}
}
