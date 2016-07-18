using UnityEngine;
using System.Collections;

public class CapItem : MicroItem{
	//The Vector3 we want to animate along once we are pulled up close
	public Vector3 animDelta;
	private float angleDeviation = 25f;
	private bool complete=false;
	public override void StartItem(){
		complete=false;
	}
	public override void OnComplete(){
		LeanTween.cancel(gameObject);
	}
	void OnDrag(DragGesture gesture){
		if (gesture.StartSelection==null||complete){
			return;
		}
		Vector3 startPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.StartPosition);
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
		Vector3 deltaPos = currentPos - startPos;
		if(Vector3.Angle(deltaPos, animDelta) < angleDeviation && gesture.StartSelection.Equals(gameObject)){
			complete=true;
			LeanTween.move(gameObject,(transform.position+animDelta),.5f).setEase(LeanTweenType.easeOutQuad);
			parent.SetWon(true);

		}
	}

}
