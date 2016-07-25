using UnityEngine;
using System.Collections;

public class PerfumeItem : MicroItem{
	public Vector3 aim;
	private Vector3 velocity;
	private float speed = .12f;
	private bool started = false;

	public override void StartItem(){
		velocity = Vector3.zero;
	}

	public void Setup(Vector3 startPos, Vector3 aim){
		transform.position = startPos;
		velocity = (aim - startPos).normalized;
		started = true;
		this.aim=aim;
	}

	void Update(){
		if(started && !MicroMixManager.Instance.IsPaused && !MicroMixManager.Instance.IsTutorial){
			transform.position += velocity * speed;
		}
	}
}
