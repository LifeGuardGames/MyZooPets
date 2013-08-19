using UnityEngine;
using System.Collections;

public class ObjectLookUp : MonoBehaviour {
	// Update is called once per frame
	void Update(){
		Vector3 eulerAngles = gameObject.transform.localEulerAngles;
		//gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
		gameObject.transform.rotation = Quaternion.Euler( new Vector3(eulerAngles.x,Quaternion.LookRotation(new Vector3(0, 1, 0)).y, eulerAngles.z));
	}
}
