using UnityEngine;
using System.Collections;

public class RoachTrapItem : MicroItem{
	public RoachItem roach;
	private bool landed = false;
	private bool complete = false;
	private bool falling;
	private float size;
	private float lastPauseTime = -1;

	public override void StartItem(){
		size = .7f;
		landed = false;
		complete = false;
		falling = false;
		SetVisible(false);
	}

	public override void OnComplete(){
	}

	public void SetVisible(bool visible){
		foreach(SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>()){
			spriteRenderer.enabled = visible;
		}
	}

	void OnTap(TapGesture gesture){
		if(Time.time - lastPauseTime > .05f){ //If we are paused right now or very recently
			StartCoroutine(CheckPlace(gesture));
		}
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			lastPauseTime = Time.time;
			return;
		}
		if(!complete && landed && Vector2.Distance(roach.transform.position, transform.position) < 1f){
			parent.SetWon(true);
			complete = true;
			roach.Freeze(transform.position);
		}
	}


	private IEnumerator CheckPlace(TapGesture gesture){
		yield return 0; //Before we place, wait a single frame to check if we clicked on the pause and are now paused after the click
		if(!falling && !MicroMixManager.Instance.IsPaused){
			Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.Position, 0);
			falling = true;
			transform.position = currentPos;
			GetComponent<Animator>().Play("Activate",0,0);
			SetVisible(true);
			landed=true;
		}
	}


}
