using UnityEngine;
using System.Collections;

public class CapItem : MicroItem{
	//The Vector3 we want to animate along once we are pulled up close
	public Vector3 animDelta;
	private float angleDeviation = 25f;
	private bool complete = false;

	public override void StartItem(){
		complete = false;
	}

	public override void OnComplete(){
		LeanTween.cancel(gameObject);
	}

	void OnDrag(DragGesture gesture){
		if(MicroMixManager.Instance.IsTutorial || gesture.StartSelection != gameObject || complete || MicroMixManager.Instance.IsPaused){
			return;
		}
		Vector3 startPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.StartPosition, 50);
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.Position, 50);
		Vector3 deltaPos = currentPos - startPos;
		if(Vector3.Angle(deltaPos, animDelta) < angleDeviation){
			complete = true;
			LeanTween.move(gameObject, (transform.position + animDelta), .5f).setEase(LeanTweenType.easeOutQuad);
			parent.SetWon(true);
		}
	}
}
