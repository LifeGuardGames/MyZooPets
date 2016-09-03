using UnityEngine;
using System.Collections;

public class FireItem : MicroItem{
	public GameObject scaleParent;
	public Sprite fireOff;
	public Sprite fireOn;
	private bool complete;
	private bool started = false;
	private float maxSize = 1.25f;
	private float shrinkSize = 1.15f;
	private float shrinkTime = .1f;
	// Use this for initialization

	public override void StartItem(){
		complete = false;
		started = false;
		GetComponent<SpriteRenderer>().sprite = fireOff;
		GetComponent<Animation>().Play();
	}

	public override void OnComplete(){

	}

	public void Engage(){
		GetComponent<Animation>().Stop();
		GetComponent<SpriteRenderer>().sprite = fireOn;
		LeanTween.scale(scaleParent, new Vector3(shrinkSize, shrinkSize), shrinkTime).setEase(LeanTweenType.easeInOutQuad);
		FireMicro fm = GetComponentInParent<FireMicro>(); //Called when we have not been set up in tutorial so we just use this instead
		fm.StartBar();
	}

	public void Disengage(){
		GetComponent<SpriteRenderer>().sprite = fireOff;
		LeanTween.scale(scaleParent, new Vector3(maxSize, maxSize), shrinkTime).setEase(LeanTweenType.easeInOutQuad);
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
