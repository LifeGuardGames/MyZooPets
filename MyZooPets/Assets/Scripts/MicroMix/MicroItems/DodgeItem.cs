using UnityEngine;
using System.Collections;

public class DodgeItem : MicroItem{
	private bool complete = false;
	private float speed = .3f;
	private float startTime;

	public override void StartItem(){
		complete = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		transform.position = Vector3.zero;
		startTime=Time.time;
	}

	void OnTriggerEnter(Collider other){
		if(!complete && other.CompareTag("MicroMixPerfume")){
			complete = true;
			parent.SetWon(false);
		}
	}
	void Update(){
		if (complete){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, Input.mousePosition);
		transform.position = Vector3.MoveTowards(transform.position, currentPos, speed);

	}
	void OnDrag(DragGesture gesture){
		/*if(complete||gesture.StartTime<startTime){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
		transform.position = Vector3.MoveTowards(transform.position, currentPos, speed);*/
	}
}
