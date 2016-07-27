using UnityEngine;
using System.Collections;

public class MoldItem : MicroItem{
	public GameObject trashItem;

	private bool complete;

	public override void StartItem(){
	}

	public override void OnComplete(){
	}

	void Update(){
		if(Vector3.Distance(transform.position, trashItem.transform.position) < 1f && !MicroMixManager.Instance.IsPaused && !MicroMixManager.Instance.IsTutorial){
			MoldMicro dm = (MoldMicro)parent;
			dm.Cleaned();
			gameObject.SetActive(false);
		}
	}
}
