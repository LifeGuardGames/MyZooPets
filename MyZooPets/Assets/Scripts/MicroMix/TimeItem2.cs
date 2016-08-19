using UnityEngine;
using System.Collections;

public class TimeItem2 : MicroItem{
	public GameObject sun;
	public GameObject clock1;
	public GameObject clock2;
	private float validDist = 1f;
	private bool hitOne = false;
	private bool pressing = false;
	private float size = 2f;
	private float shrinkTime = .15f;
	//If we got the first correct
	public override void StartItem(){
		clock1.SetActive(true);
		clock2.SetActive(true);
		hitOne = false;
		pressing = false;
		gameObject.transform.localScale = new Vector3(size, size, 1f);
	}

	public override void OnComplete(){
		LeanTween.cancel(gameObject);
	}

	public void ShrinkDown(){
		LeanTween.scale(gameObject, new Vector3(size - .2f, size - .2f), shrinkTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(GrowBack);
	}

	void OnTap(TapGesture gesture){
		if(gesture.Selection != gameObject || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || pressing){
			return;
		}
		ShrinkDown();
		pressing = true;
		if(Vector3.SqrMagnitude(clock1.transform.position - sun.transform.position) < validDist){
			clock1.SetActive(false);
			hitOne = true;
		}
		if(Vector3.SqrMagnitude(clock2.transform.position - sun.transform.position) < validDist){
			clock2.SetActive(false);
			if(hitOne){
				parent.SetWon(true);
			}
		}
	}

	private void GrowBack(){
		LeanTween.scale(gameObject, new Vector3(size, size), shrinkTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(FinishPress);
	}

	private void FinishPress(){
		pressing = false;
	}
}
