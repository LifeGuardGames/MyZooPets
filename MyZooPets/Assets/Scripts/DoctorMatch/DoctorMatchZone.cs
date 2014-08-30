using UnityEngine;
using System.Collections;

public class DoctorMatchZone : MonoBehaviour {

	public string zoneKey;	// Key to match with the item key
	public Vector3 hoverScale;
	private Collider2D auxItemCheck;
	private string auxItemCheckKey;
	private bool activeState;
	public ParticleSystem particle;
	public GameObject floatyPrefab;

	void Start(){
		if(zoneKey == null){
			Debug.LogError("Zone key is missing");
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		SetActiveState(true);
		auxItemCheck = collider;
		auxItemCheckKey = collider.GetComponent<AssemblyLineItem>().itemKey;

		AssemblyLineItem item = collider.gameObject.GetComponent<AssemblyLineItem>();
		item.SetHoverZoneKey(zoneKey);
	}

	// Some descrepancy between trigger enter and trigger exit, do brute force check every frame for active
	void OnTriggerStay2D(Collider2D collider){
		SetActiveState(true);

		AssemblyLineItem item = collider.gameObject.GetComponent<AssemblyLineItem>();
		item.SetHoverZoneKey(zoneKey);
	}

	void OnTriggerExit2D(Collider2D collider){
		SetActiveState(false);

		AssemblyLineItem item = collider.gameObject.GetComponent<AssemblyLineItem>();
		item.SetHoverZoneKey(null);
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
			// Local check here as well as in assembly line item
			if(auxItemCheckKey == zoneKey){
				GameObject go = Instantiate(floatyPrefab) as GameObject;
				go.transform.parent = gameObject.transform;
				go.transform.localPosition = Vector3.zero;
				particle.Play();
			}
			SetActiveState(false);
			auxItemCheckKey = null;
		}
	}
}
