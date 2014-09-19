using UnityEngine;
using System.Collections;

public static class GameObjectUtils {

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
}
