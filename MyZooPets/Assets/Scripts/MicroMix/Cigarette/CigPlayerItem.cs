using UnityEngine;

public class CigPlayerItem : MicroItem{
	public Vector3 finishPos;
	public bool complete;
	private float speed = .3f;

	public override void StartItem(){
		complete = false;
	}

	public override void OnComplete(){
		
	}

	void Update(){
		if(complete || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		if(Input.GetMouseButton(0)){
			Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, Input.mousePosition, 0);
			transform.position = Vector3.MoveTowards(transform.position, currentPos, speed);
		}
		if(!complete && Vector3.Distance(transform.position, finishPos) < 1f){
			parent.SetWon(true);
			complete = true;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if(!complete && other.CompareTag("MicroMixPerfume")){
			complete = true;
		}
	}
}

