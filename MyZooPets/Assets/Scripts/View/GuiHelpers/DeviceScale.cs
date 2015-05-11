using UnityEngine;
using System.Collections;

// Scales gameobjects for recognizing different devices
// Since NGUI does its own scaling, we just do it by eye on the object rather than calculated
public class DeviceScale : MonoBehaviour {
	void Start () {
		gameObject.transform.localScale = new Vector3((float)CameraManager.Instance.NativeWidth, 768f, 1f);
	}
}
