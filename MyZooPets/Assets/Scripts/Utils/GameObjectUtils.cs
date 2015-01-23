using UnityEngine;
using System.Collections;

public static class GameObjectUtils{

	static public void ResetLocalTransform(GameObject go){
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}

	static public void ResetLocalPosition(GameObject go){
		go.transform.localPosition = Vector3.zero;
	}

	static public void ResetLocalScale(GameObject go){
		go.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Instantiate an object and add it to the specified parent.
	/// </summary>
	static public GameObject AddChild(GameObject parent, GameObject prefab){
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		
		if(go != null && parent != null){
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}
}
