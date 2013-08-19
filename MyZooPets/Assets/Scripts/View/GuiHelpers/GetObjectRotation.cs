using UnityEngine;
using System.Collections;

public class GetObjectRotation : MonoBehaviour {

	public GameObject rotationObject;

	public bool inheritX = false;
	public bool inheritY = false;
	public bool inheritZ = false;

	// Update is called once per frame
	void Update () {
		if(inheritX || inheritY || inheritZ){
			float xRotation = inheritX ? rotationObject.transform.localEulerAngles.x : gameObject.transform.localEulerAngles.x;
			float yRotation = inheritY ? rotationObject.transform.localEulerAngles.y : gameObject.transform.localEulerAngles.y;
			float zRotation = inheritZ ? rotationObject.transform.localEulerAngles.z : gameObject.transform.localEulerAngles.z;

			gameObject.transform.localEulerAngles = new Vector3(xRotation, yRotation, zRotation);
		}
	}
}
