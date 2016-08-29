using UnityEngine;
using System.Collections;

public class CampfireItem : MicroItem{
	public GameObject[] logs;
	public ParticleSystem smokeSystem;
	public GameObject colliderGO;
	private float smokeSpeed = 10;

	private float currentTime;
	private float nextTime;
	private float minDelay = .8f;
	private float maxDelay = 1.2f;

	//In radians
	private float windDirection;
	private float windDelta;
	private float maxWindDelta = Mathf.PI / 2;

	void Update(){
		if(MicroMixManager.Instance.IsPaused){
			return;
		}

		currentTime += Time.deltaTime;
		windDirection += windDelta * Time.deltaTime;
		smokeSystem.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * windDirection));
			
		if(currentTime > nextTime){
			currentTime = 0;
			nextTime = Random.Range(minDelay, maxDelay);
			windDelta = Random.Range(-maxWindDelta, maxWindDelta);
		}
	}

	public float GetCurrentRadians(){
		return windDirection;
	}

	public override void StartItem(){
		for(int i = 0; i < logs.Length; i++){
			logs[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.value * 360f));
			logs[i].transform.localScale = new Vector3(Random.Range(.8f, 1.2f), Random.Range(.8f, 1.2f), 1);
		}
		currentTime = 0;
		nextTime = Random.Range(minDelay, maxDelay);
		windDirection = Random.value * 2 * Mathf.PI;
		windDelta = Random.Range(-maxWindDelta, maxWindDelta);
	}

	public override void OnComplete(){

	}

}
