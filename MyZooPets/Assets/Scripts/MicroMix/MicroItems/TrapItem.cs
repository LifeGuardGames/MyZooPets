using UnityEngine;
using System.Collections;

public class TrapItem : MicroItem{
	public GameObject shadow;
	public GameObject roach;
	private bool landed=false;
	private bool complete=false;
	private bool falling;
	private float size;

	public override void StartItem(){
		shadow.GetComponent<Renderer>().enabled = false;
		GetComponent<Renderer>().enabled = false;
		size = .7f;
		landed=false;
		complete=false;
		falling=false;
	}

	void OnTap(TapGesture gesture){
		if(!falling){
			Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
			falling = true;
			transform.position=currentPos;
			StartCoroutine(Fall());
		}
	}
	void Update(){
		if (!complete&&landed&&Vector3.Distance(roach.transform.position,transform.position)<1f){
			parent.SetWon(true);
			complete=true;
			roach.SetActive(false);
		}
	}
	private IEnumerator Fall(){
		shadow.GetComponent<Renderer>().enabled = true;
		transform.localScale = new Vector3(size, size, size);
		for(int i = 0; i < 3; i++){
			yield return new WaitForSeconds(.1f);
			size += .2f;
			transform.localScale = new Vector3(size, size, size);
		}
		GetComponent<Renderer>().enabled=true;
		landed=true;
	}
}
