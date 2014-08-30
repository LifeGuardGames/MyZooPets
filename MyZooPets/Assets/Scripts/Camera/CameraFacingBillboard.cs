using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour{
	public new Camera camera;

	void Start(){
		if(!camera)
			camera = GameObject.Find("Main Camera").camera;
		transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
            camera.transform.rotation * Vector3.up);
	}

	void Update(){
		transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
            camera.transform.rotation * Vector3.up);
	}
}