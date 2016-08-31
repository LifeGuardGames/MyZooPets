using UnityEngine;
using System.Collections;

public class CampCollectItem : MicroItem{
	public override void StartItem(){
		float angle = Random.value * Mathf.PI * 2;
		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * CampMicro.distance;
	}

	public override void OnComplete(){
		
	}
}
