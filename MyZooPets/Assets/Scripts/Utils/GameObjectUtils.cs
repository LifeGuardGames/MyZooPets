using UnityEngine;
using System.Collections;

public static class GameObjectUtils {

	static public void ZeroLocalTransform(GameObject go){
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}

}
