using UnityEngine;
using System.Collections;

public class FireItem : MicroItem{
	public Sprite fireOff;
	public Sprite fireOn;
	private bool complete;
	private bool started = false;
	private float shrinkSize = .95f;
	private float shrinkTime = .1f;
	// Use this for initialization
	public override void StartItem(){
		complete = false;
		started = false;
		GetComponent<SpriteRenderer>().sprite = fireOff;
	}

	public override void OnComplete(){

	}

	public void Engage(){
		GetComponent<SpriteRenderer>().sprite = fireOn;
		LeanTween.scale(gameObject, new Vector3(shrinkSize, shrinkSize), shrinkTime).setEase(LeanTweenType.easeInOutQuad);
		FireMicro fm = GetComponentInParent<FireMicro>(); //Called when we have not been set up in tutorial so we just use this instead
		fm.StartBar();
	}

	public void Disengage(){
		GetComponent<SpriteRenderer>().sprite = fireOff;
		LeanTween.scale(gameObject, new Vector3(1, 1), shrinkTime).setEase(LeanTweenType.easeInOutQuad);
		FireMicro fm = GetComponentInParent<FireMicro>();
		fm.StopBar();
	}

	void OnMouseDown(){
		if(!complete && !MicroMixManager.Instance.IsPaused && !MicroMixManager.Instance.IsTutorial){
			complete = true;
			started = true;
			Engage();
		}
	}

	void OnMouseUp(){
		if(started && !MicroMixManager.Instance.IsPaused && !MicroMixManager.Instance.IsTutorial){
			Disengage();
			FireMicro fm = (FireMicro)parent;
			started = false;
			parent.SetWon(fm.IsCorrect());
		}
	}
		
}
