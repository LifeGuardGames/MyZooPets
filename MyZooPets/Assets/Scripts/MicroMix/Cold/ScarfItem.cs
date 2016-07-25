using UnityEngine;
using System.Collections;

public class ScarfItem : MicroItem{
	public GameObject pet;
	private bool complete;
	private Transform scarfTransform;
	private Vector3 neckOffset = new Vector3(.17f, .5f);
	private Vector3 posOffset = Vector3.zero;
	//How far off to move from currentPos;

	//How far off to move from pet.transform.position

	public override void StartItem(){
		complete = false;
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection != gameObject || complete || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || !pet){
			return;
		}
		if(posOffset == Vector3.zero){
			Vector3 startPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.StartPosition);
			posOffset = transform.position - startPos;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
		transform.position = currentPos + posOffset;
		Vector3 mouthPos = pet.transform.position + neckOffset;

		if(Vector3.Distance(transform.position, mouthPos) < .5f){
			parent.SetWon(true);
			transform.position = mouthPos;
			complete = true;
			pet.GetComponentInChildren<Animator>().SetTrigger("InhalerHappy1");
		}
	}

	void Update(){
		if(complete && !MicroMixManager.Instance.IsPaused && pet){
			transform.position=pet.transform.position+neckOffset;
		}
	}
}
