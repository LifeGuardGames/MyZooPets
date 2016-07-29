using UnityEngine;
using System.Collections;

public class ScarfItem : MicroItem{
	public GameObject pet;
	private bool complete;
	private Vector3 posOffset = Vector3.zero;
	private GameObject neckObject;

	public override void StartItem(){
		complete = false;
		neckObject = pet.GetComponent<MicroMixAnatomy>().neck;
	}

	public override void OnComplete(){
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection != gameObject || complete || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || !pet){
			return;
		}
		if(posOffset == Vector3.zero){
			Vector3 startPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.StartPosition, 50);
			posOffset = transform.position - startPos;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position, 50);
		transform.position = currentPos + posOffset;
		Vector3 neckPos = neckObject.transform.position;
		if(Vector3.Distance(transform.position, neckPos) < .75f){
			parent.SetWon(true);
			transform.position = neckPos;
			complete = true;
			pet.GetComponentInChildren<Animator>().SetTrigger("InhalerHappy1");
		}
	}

	void Update(){
		if(complete && !MicroMixManager.Instance.IsPaused && pet){
			transform.position = neckObject.transform.position;
		}
	}
}
