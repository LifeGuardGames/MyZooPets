using UnityEngine;
using System.Collections;

public class SpacerItem : MicroItem{
	public GameObject inhaler;
	private bool complete;

	public override void StartItem(){
		complete = false;
	}

	public override void OnComplete(){
	}

	void OnDrag(DragGesture gesture){
		if(MicroMixManager.Instance.IsTutorial || gesture.StartSelection != gameObject || complete || MicroMixManager.Instance.IsPaused){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position, 50);
		transform.position = currentPos;
		if(Vector3.Distance(transform.position, inhaler.transform.position) < .25f){
			complete = true;
			parent.SetWon(true);
			transform.position = inhaler.transform.position;
		}
	}
}
