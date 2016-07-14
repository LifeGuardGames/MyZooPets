using UnityEngine;
using System.Collections;

public class ButtonItem : MicroItem{
	public Micro parent;
	private Vector3 animDelta = new Vector3(0, -.5f);
	private bool complete = false;

	public override void StartItem(){
		complete = false;
	}

	public override void OnComplete(){
		LeanTween.cancel(gameObject);
	}

	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null || complete){
			return;
		}
		if(gesture.StartSelection.Equals(gameObject)){
			complete = true;
			LeanTween.move(gameObject, (transform.position + animDelta), .5f).setEase(LeanTweenType.easeInOutQuad);
			parent.SetWon(true);
		}
	}
}
