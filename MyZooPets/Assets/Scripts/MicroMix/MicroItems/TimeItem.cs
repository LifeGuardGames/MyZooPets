using UnityEngine;
using System.Collections;
using UnityEditor;

public class TimeItem : MicroItem{
	public GameObject petInstance;
	private bool complete = false;
	private float timeStart = Time.time;
	public override void StartItem(){
		complete = false;
		timeStart = Time.time;
	}

	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null || complete || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || (Time.time-timeStart)<.3f){
			return;
		}
		else if(gesture.StartSelection.Equals(gameObject)){
			complete = true;
			TimeMicro tm = (TimeMicro)parent;
			if(tm.IsValid()){
				parent.SetWon(true);
				petInstance.GetComponentInChildren<Animator>().SetTrigger("InhalerHappy1");
			}
			LeanTween.scale(gameObject,Vector3.one*3.5f,.2f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(ScaleBack);
		}
	}
	private void ScaleBack(){
		LeanTween.scale(gameObject,Vector3.one*4,.2f).setEase(LeanTweenType.easeInOutQuad);
	}
}
