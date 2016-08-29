using UnityEngine;
using System.Collections;

public class MowerItem : MicroItem{
	private float screenOrientationMultiplier = 1;
	private float angle;
	//In radians
	private float angularSpeed = Mathf.PI;
	private float maxTilt = .5f;
	//Pi/2
	private float speed = 5f;
	private bool canFlip = true;

	void Update(){
		if(MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		angle += screenOrientationMultiplier * Mathf.Clamp(Input.acceleration.x, -maxTilt, maxTilt) * angularSpeed * Time.deltaTime; 
		transform.position -= new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * speed * Time.deltaTime;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg));
		if(canFlip){
			CheckBounds();
		}
		else{
			canFlip = true;
		}

	}

	void OnTriggerEnter2D(Collider2D other){
		if(parent == null){
			return;
		}
		((GrassMicro)parent).RemoveGrass(other.gameObject);
	}

	public override void StartItem(){
		//screenOrientationMultiplier = (Screen.orientation == ScreenOrientation.LandscapeRight) ? 1 : -1;
		Debug.LogWarning("Screen orientation not accounted for");
	}

	public override void OnComplete(){
		
	}

	private void CheckBounds(){
		Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		float x = transform.position.x;
		float y = transform.position.y;
		if(screenPos.x > Screen.width || screenPos.x < 0){
			x *= -1;
			canFlip = false;
		}
		if(screenPos.y > Screen.height || screenPos.y < 0){
			y *= -1;
			canFlip = false;
		}
		transform.position = new Vector3(x, y);
	}

}

