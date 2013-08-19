using UnityEngine;
using System.Collections;

public class LockRotation : MonoBehaviour {

	public bool lockX;
	public float overrideX;

	public bool lockY;
	public float overrideY;

	public bool lockZ;
	public float overrideZ;

	void Update(){
		if(lockX || lockY || lockZ){
			float xRotation = lockX ? overrideX : gameObject.transform.eulerAngles.x;
			float yRotation = lockY ? overrideY : gameObject.transform.eulerAngles.y;
			float zRotation = lockZ ? overrideZ : gameObject.transform.eulerAngles.z;

			gameObject.transform.eulerAngles = new Vector3(xRotation, yRotation, zRotation);
		}
	}
}
