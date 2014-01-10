using UnityEngine;
using System.Collections;

// Scales gameobjects for recognizing different devices
// Since NGUI does its own scaling, we just do it by eye on the object rather than calculated
public class DeviceScale : MonoBehaviour {
	
	public Vector3 iPhone5Scale;
	public Vector3 iPhone4Scale;
	
	void Start () {
		if(iPhone.generation == iPhoneGeneration.iPhone5 || (Screen.width == 1136 && Screen.height == 640)){
			gameObject.transform.localScale = iPhone5Scale;
		}
		if(iPhone.generation == iPhoneGeneration.iPhone4 || iPhone.generation == iPhoneGeneration.iPhone4S || (Screen.width == 960 && Screen.height == 640)){
			gameObject.transform.localScale = iPhone4Scale;
		}
	}
}
