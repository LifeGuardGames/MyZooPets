using UnityEngine;
using System.Collections;

public class ShakeItem : MicroItem{
	private float moved = 0;
	private bool complete = false;

	public override void StartItem(){
		moved = 0;
		complete = false;
	}

	public override void OnComplete(){
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection == null || complete || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.Position, 0);
		transform.position = currentPos;
		moved += gesture.DeltaMove.magnitude;
		if(moved > 1000){
			parent.SetWon(true);
			complete = true;
		}
	}
}
