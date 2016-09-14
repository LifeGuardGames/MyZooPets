using UnityEngine;
using System.Collections;

public class CampPlayerItem : MicroItem{
	//In radians
	private float angle;
	private float maxTilt = .5f;
	private float angularSpeed = Mathf.PI * 2;
	private float screenOrientationMultiplier = -1;
	private bool complete = false;

	void Update(){
		if(MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || complete){
			return;
		}
		if(Input.GetKey("left")) {
			Debug.Log("hi");
			//	angle += screenOrientationMultiplier * Mathf.Clamp(Input.acceleration.x, -maxTilt, maxTilt) * angularSpeed * Time.deltaTime; 
			angle += 1f * angularSpeed * Time.deltaTime;
		}
		if(Input.GetKey("right")) {
			Debug.Log("hi");
			//	angle += screenOrientationMultiplier * Mathf.Clamp(Input.acceleration.x, -maxTilt, maxTilt) * angularSpeed * Time.deltaTime; 
			angle += -1f * angularSpeed * Time.deltaTime;
		}
		else {
			angle += 0 * Mathf.Clamp(Input.acceleration.x, -maxTilt, maxTilt) * angularSpeed * Time.deltaTime;
		}
		//angle += screenOrientationMultiplier * Mathf.Clamp(Input.acceleration.x, -maxTilt, maxTilt) * angularSpeed * Time.deltaTime; 
		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * CampMicro.distance;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(parent == null){
			if(other.CompareTag("MicroMixCollect")&&MicroMixManager.Instance.IsTutorial){
				other.gameObject.SetActive(false);
			}
			return;
		}
		Debug.Log(other.tag);
		if(other.CompareTag("MicroMixCollect")){
			((CampMicro)parent).Collect(other.gameObject);
		}
		else if(other.CompareTag("MicroMixPerfume")){
			complete = true;
		}
	}

	public override void StartItem(){
		Debug.LogWarning("Screen orientation not accounted for");
		complete = false;
	}

	public void RotateTowards(float aimRadians, float time){
		LeanTween.value(gameObject, SetAngle, angle, aimRadians, time);
	}

	public override void OnComplete(){
		
	}

	public void SetAngle(float angle){
		this.angle = angle;
		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * CampMicro.distance;
	}
}
