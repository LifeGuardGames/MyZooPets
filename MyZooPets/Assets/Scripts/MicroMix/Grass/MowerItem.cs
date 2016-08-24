using UnityEngine;
using System.Collections;

public class MowerItem : MonoBehaviour {
	private float angle; //In radians
	void Update(){
		angle+=Input.acceleration.x*Time.deltaTime;
		transform.position+=new Vector3(Mathf.Cos(angle),Mathf.Sin(angle));
		transform.rotation = Quaternion.Euler(new Vector3(0,0,angle*Mathf.Rad2Deg));
	}
}
