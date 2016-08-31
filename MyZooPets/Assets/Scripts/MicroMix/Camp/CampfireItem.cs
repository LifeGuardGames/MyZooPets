using UnityEngine;
using System.Collections;

public class CampFireItem : MicroItem{
	public GameObject[] logs;
	public ParticleSystem smokeSystem;
	private bool stopped;

	private float currentTime;
	private float nextTime;
	private float minDelay = 1f;
	private float maxDelay = 1.4f;

	//In radians
	private float windDirection;
	private float windDelta;
	private float minWindDelta = Mathf.PI / 4;
	private float maxWindDelta = Mathf.PI / 3;

	void Update(){
		if(MicroMixManager.Instance.IsPaused || stopped){
			return;
		}

		currentTime += Time.deltaTime;
		windDirection += windDelta * Time.deltaTime;
		smokeSystem.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * windDirection));

		if(currentTime > nextTime){
			currentTime = 0;
			nextTime = Random.Range(minDelay, maxDelay);
			windDelta = Random.Range(minWindDelta, maxWindDelta);
			windDelta *= (Random.value > .5f) ? 1 : -1;
		}
	}

	public void Stop(){
		stopped = true;
	}

	public void RotateTowards(float aimRadians, float time){
		LeanTween.value(gameObject, SetAngle, windDirection, aimRadians, time);
	}

	public override void StartItem(){
		for(int i = 0; i < logs.Length; i++){
			logs[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.value * 360f));
			logs[i].transform.localScale = new Vector3(Random.Range(.8f, 1.2f), Random.Range(.8f, 1.2f), 1);
		}
		currentTime = 0;
		nextTime = Random.Range(minDelay, maxDelay);
		windDelta = Random.Range(-maxWindDelta, maxWindDelta);
		stopped = false;
	}

	public override void OnComplete(){

	}

	public void SetAngle(float windDirection){
		this.windDirection = windDirection;
		smokeSystem.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * windDirection));
	}
}