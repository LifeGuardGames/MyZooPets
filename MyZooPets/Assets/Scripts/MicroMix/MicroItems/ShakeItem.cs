using UnityEngine;
using System.Collections;

public class ShakeItem : MicroItem {
	private float moved=0;
	private bool complete=false;
	public override void StartItem(){
		moved=0;
		complete=false;
	}
	void OnDrag(DragGesture gesture){
		if (gesture.StartSelection==null||complete){
			return;
		}
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main,gesture.Position);
		transform.position=currentPos;
		Debug.Log(moved);
		moved+=gesture.DeltaMove.magnitude;
		if (moved>1000){
			parent.SetWon(true);
			complete=true;
		}
	}
}
