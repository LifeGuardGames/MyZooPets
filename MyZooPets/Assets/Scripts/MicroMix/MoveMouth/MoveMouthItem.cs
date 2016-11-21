using UnityEngine;
using System.Collections;

public class MoveMouthItem : MicroItem{
	public GameObject pet;
	private GameObject mouthObject;
	private bool complete;
	private Vector3 posOffset = Vector3.zero;
	//How far off to move from currentPos;
	public override void StartItem(){
		complete = false;
		mouthObject = pet.GetComponent<MicroMixAnatomy>().mouth;
	}

	public override void OnComplete(){
	}

	void OnDrag(DragGesture gesture){
		if(MicroMixManager.Instance.IsTutorial || gesture.StartSelection != gameObject || complete || MicroMixManager.Instance.IsPaused){
			return;
		}
		if(posOffset == Vector3.zero){
			Vector3 startPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.StartPosition, 0);
			posOffset = transform.position - startPos;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.Position, 0);
		transform.position = currentPos + posOffset;
		if(Vector3.Distance(transform.position, mouthObject.transform.position) < 1){
			complete = true;
			parent.SetWon(true);
			transform.position = mouthObject.transform.position;
		}
	}
}
