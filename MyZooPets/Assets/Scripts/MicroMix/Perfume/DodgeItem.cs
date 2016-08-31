using UnityEngine;
using System.Collections;

public class DodgeItem : MicroItem{
	public bool hit = false;
	private float speed = .3f;
	private float startTime;

	public override void StartItem(){
		hit = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		startTime = Time.time;
	}

	public override void OnComplete(){
	}

	void OnTriggerEnter(Collider other){
		if(!hit && other.CompareTag("MicroMixPerfume")||MicroMixManager.Instance.IsPaused||parent){
			hit = true;
			parent.SetWon(false);
		}
	}

	void Update(){
		if(hit || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, Input.mousePosition, 0);
		transform.position = Vector3.MoveTowards(transform.position, currentPos, speed);
	}

}
