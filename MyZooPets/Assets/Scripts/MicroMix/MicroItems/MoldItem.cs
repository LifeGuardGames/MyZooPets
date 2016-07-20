using UnityEngine;
using System.Collections;

public class MoldItem : MicroItem{
	private bool complete;

	public override void StartItem(){
	}

	void Update(){
		if(Vector3.Distance(transform.position, Vector3.zero) < .8f){
			MoldMicro dm = (MoldMicro)parent;
			dm.Cleaned();
			gameObject.SetActive(false);
		}
	}
}
