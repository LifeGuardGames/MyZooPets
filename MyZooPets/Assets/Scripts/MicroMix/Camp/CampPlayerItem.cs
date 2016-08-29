using UnityEngine;
using System.Collections;

public class CampPlayerItem : MicroItem{
	//In radians
	private float angle;
	private float maxTilt = .5f;
	private float angularSpeed = Mathf.PI * 2;
	private float screenOrientationMultiplier = -1;


	void Update(){
		if(MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		angle += screenOrientationMultiplier * Mathf.Clamp(Input.acceleration.x, -maxTilt, maxTilt) * angularSpeed * Time.deltaTime; 
		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 5;
	}

	public override void StartItem(){
		angle = 0;
		Debug.LogWarning("Screen orientation not accounted for");
	}

	public override void OnComplete(){
		
	}
}
