using UnityEngine;
using System.Collections;

public class FireItem : MicroItem{
	public GameObject mover;
	private bool complete;
	private bool holding = false;
	private bool started = false;

	public override void StartItem(){
		complete = false;
		started = false;
	}

	public override void OnComplete(){
		
	}

	void OnMouseDown(){
		if(!complete){
			FireMicro fm = (FireMicro)parent;
			fm.StartMover();
			complete = true;
			started = true;

		}
	}

	void OnMouseUp(){
		if(started){
			FireMicro fm = (FireMicro)parent;
			fm.StopMover();
			started = false;
			parent.SetWon(Correct());
			Debug.Log(Correct());
		}
	}

	private bool Correct(){
		float y = mover.transform.position.y;
		return Mathf.Abs(y) < .3f;
	}
}
