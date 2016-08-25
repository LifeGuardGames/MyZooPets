using UnityEngine;
using System.Collections;

public class DodgeItem : MicroItem{
	public bool complete = false;
	private float speed = .3f;
	private float startTime;

	public override void StartItem(){
		complete = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		transform.position = Vector3.zero + Vector3.forward * 50;
		startTime = Time.time;
	}

	public override void OnComplete(){
	}

	void OnTriggerEnter(Collider other){
		if(!complete && other.CompareTag("MicroMixPerfume")){
			complete = true;
			parent.SetWon(false);
		}
	}

	void Update(){
		if(complete || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, Input.mousePosition, 0);
		transform.position = Vector3.MoveTowards(transform.position, currentPos, speed);
	}

}
