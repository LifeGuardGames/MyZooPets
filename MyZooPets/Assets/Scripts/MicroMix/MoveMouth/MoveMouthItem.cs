using UnityEngine;
using System.Collections;

public class MoveMouthItem : MicroItem{
	public GameObject pet;
	private bool complete;
	private Vector3 mouthOffset = new Vector3(.5f,1f); //How far off to move from pet.transform.position
	private Vector3 posOffset = Vector3.zero; //How far off to move from currentPos;
	public override void StartItem(){
		complete = false;
	}

	void OnDrag(DragGesture gesture){
		if(MicroMixManager.Instance.IsTutorial || gesture.StartSelection != gameObject || complete || MicroMixManager.Instance.IsPaused){
			return;
		}
		if (posOffset==Vector3.zero){
			Vector3 startPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.StartPosition);
			posOffset = transform.position-startPos;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
		transform.position = currentPos + posOffset;
		Vector3 mouthPos = pet.transform.position+mouthOffset;
		if(Vector3.Distance(transform.position, mouthPos) < 1){
			complete = true;
			parent.SetWon(true);
			transform.position = mouthPos;
		}
	}
}
