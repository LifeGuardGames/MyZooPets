using UnityEngine;
using System.Collections;

public class ObjectLookUp : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
	}
}
