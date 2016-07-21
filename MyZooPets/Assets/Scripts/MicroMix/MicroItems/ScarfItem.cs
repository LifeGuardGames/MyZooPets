using UnityEngine;
using System.Collections;

public class ScarfItem : MicroItem{
	private bool complete;
	private Transform scarfTransform;

	public override void StartItem(){
		complete = false;
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection == null || complete || MicroMixManager.Instance.IsPaused){
			return;
		}
		if(gesture.StartSelection.gameObject != this){
			Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
			Transform selected = gesture.StartSelection.gameObject.transform;
			selected.position = currentPos;
			if(Vector3.Distance(selected.position, transform.position) < 1f){
				parent.SetWon(true);
				selected.position = transform.position;
				complete = true;
				scarfTransform = selected;
				GetComponentInParent<Animator>().SetTrigger("InhalerHappy1");
			}

		}
	}

	void Update(){
		if(complete & !MicroMixManager.Instance.IsPaused){
			scarfTransform.position = transform.position;
		}
	}
}
