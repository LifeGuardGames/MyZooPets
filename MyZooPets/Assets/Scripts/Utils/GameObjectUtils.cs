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

	/// <summary>
	/// Instantiate an object and add it to the specified parent. use the position of the prefab
	/// </summary>
	static public GameObject AddChildWithPositionAndScale(GameObject parent, GameObject prefab){
		GameObject go = AddChild(parent, prefab);
		if(go != null){
			Transform t = go.transform;
			t.localPosition = prefab.transform.localPosition;
			t.localScale = prefab.transform.localScale;
		}
		return go;
	}

	/// <summary>
	/// Instantiate an object and add it to the specified parent. use all the transforms of prefab
	/// </summary>
	static public GameObject AddChildWithTransform(GameObject parent, GameObject prefab){
		GameObject go = AddChild(parent, prefab);
		if(go != null){
			Transform t = go.transform;
			t.localPosition = prefab.transform.localPosition;
			t.localRotation = prefab.transform.localRotation;
			t.localScale = prefab.transform.localScale;
		}
		return go;
	}

	/// <summary>
	/// Gets the point on circle circumference centered around center on XY plane.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="radius">Radius.</param>
	static public Vector3 GetRandomPointOnCircumference(Vector3 center, float radius){
		float randomDegree = UnityEngine.Random.Range(0f,360f);
		float xComponent = radius * Mathf.Sin(randomDegree);	// Not sure if sin or cos but we dont really care
		float yComponent = radius * Mathf.Cos(randomDegree);
		return new Vector3(center.x + xComponent, center.y + yComponent, center.z);
	}
}
