using UnityEngine;
using System.Collections;

public class CampCollectItem : MicroItem{
	public void Randomize(){
		transform.rotation = Quaternion.Euler(0,0,Random.value*360);
	}

	public override void StartItem(){
		float angle = Random.value * Mathf.PI * 2;
		transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * CampMicro.distance;
	}

	public override void OnComplete(){
		
	}
}
