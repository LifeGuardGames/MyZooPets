using UnityEngine;
using System.Collections;

/// <summary>
/// Instead of billboarding, this actually faces the camera at all times, billboarding breaks when camera y changes..
/// </summary>
public class LookAtCamera : MonoBehaviour {
	public new Camera camera;
	
	void Start(){
		if(!camera)
			camera = GameObject.Find("Main Camera").GetComponent<Camera>();

		// Flip the camera because we are using look at which looks at the back of it..
		transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		transform.LookAt(camera.transform);
	}
	
	void Update(){
		transform.LookAt(camera.transform);
	}
}
