using UnityEngine;
using System.Collections;

public class OldFireItem : MicroItem{
	public GameObject mover;
	public GameObject barUI;
	public Sprite fireOff;
	public Sprite fireOn;
	private bool complete;
	private bool started = false;

	public override void StartItem(){
		complete = false;
		started = false;
		GetComponent<SpriteRenderer>().sprite=fireOff;
	}

	public override void OnComplete(){
		
	}

	void OnMouseDown(){
		if(!complete){
			GetComponent<SpriteRenderer>().sprite=fireOn;
			FireMicro fm = (FireMicro)parent;
			fm.StartBar();
			complete = true;
			started = true;

		}
	}

	void OnMouseUp(){
		if(started){
			GetComponent<SpriteRenderer>().sprite=fireOff;
			FireMicro fm = (FireMicro)parent;
			fm.StopBar();
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
