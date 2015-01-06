using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {

	float spped;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Time.deltaTime, 0, 0, Camera.main.transform);
	}
	void OnBecameInvisiable ()
	{
		Destroy(this.gameObject);
	}
}
