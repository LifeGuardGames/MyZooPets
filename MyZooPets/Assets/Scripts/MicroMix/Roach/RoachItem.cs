using UnityEngine;
using System.Collections;

public class RoachItem : MicroItem{
	private Vector3 lastPos;
	private Vector3 velocity;
	private float speed = 5f;
	private bool setup = false;

	public override void StartItem(){
	}

	public override void OnComplete(){
		setup = false;
	}

	public void Setup(Vector3 velocity){
		this.velocity = velocity.normalized * speed;
		setup = true;
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || !setup){
			return;
		}
		lastPos = transform.position;
		transform.position += velocity * Time.deltaTime;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		if(screenPos.x < 0 || screenPos.x > Screen.width){
			velocity = new Vector3(-1 * velocity.x, velocity.y);
			transform.position = lastPos;
		}
		if(screenPos.y < 0 || screenPos.y > Screen.height){
			velocity = new Vector3(velocity.x, -1 * velocity.y);
			transform.position = lastPos;
		}
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x) - 90));
	}
}
