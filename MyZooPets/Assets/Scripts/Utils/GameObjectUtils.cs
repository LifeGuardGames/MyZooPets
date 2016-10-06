using UnityEngine;

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

	static public void CopyRectTransform(RectTransform target, RectTransform source){
		target.anchoredPosition3D = source.anchoredPosition3D;
		target.anchorMax = source.anchorMax;
		target.anchorMin = source.anchorMin;
		target.offsetMax = source.offsetMax;
		target.offsetMin = source.offsetMin;
		target.pivot = source.pivot;
		target.sizeDelta = source.sizeDelta;
		target.localEulerAngles = source.localEulerAngles;
		target.localScale = source.localScale;
	}

	/// <summary>
	/// Instantiate an object and add it to the specified parent.
	/// </summary>
	static public GameObject AddChild(GameObject parent, GameObject prefab, bool isPreserveLayer = false){
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

		if(go != null){
			Transform t = go.transform;

			if(parent != null){
				t.SetParent(parent.transform);

				if(!isPreserveLayer){
					go.layer = parent.layer;
				}
			}
			else{
				t.parent = null;	// Assign to root
			}

			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
		}
		return go;
	}

	static public GameObject AddChildGUI(GameObject parent, GameObject prefab, bool isPreserveLayer = false){
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

		if(go != null){
			Transform t = go.transform;

			if(parent != null){
				t.SetParent(parent.transform, false);

				if(!isPreserveLayer){
					go.layer = parent.layer;
				}
			}
			else{
				t.SetParent(null);	// Assign to root
			}

			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
		}
		return go;
	}

	static public GameObject AddChildGUIWithPosition(GameObject parent, GameObject prefab) {
		GameObject go = AddChildGUI(parent, prefab);
		if(go != null) {
			RectTransform rt = prefab.GetComponent<RectTransform>();
			go.GetComponent<RectTransform>().localPosition = rt.localPosition;
		}
		return go;
	}

	/// <summary>
	/// Instantiate an object and add it to the specified parent. use the localposition and localscale of the prefab
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
	/// Instantiate an object and add it to the specified parent. use the position of the prefab
	/// </summary>
	static public GameObject AddChildWithPosition(GameObject parent, GameObject prefab) {
		GameObject go = AddChild(parent, prefab);
		if(go != null) {
			Transform t = go.transform;
			t.localPosition = prefab.transform.localPosition;
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

	static public Vector2 GetRandomPointOnCircumference(Vector2 center, float radius){
		Vector3 position = GetRandomPointOnCircumference(new Vector3(center.x, center.y, 0f), radius);
		return new Vector2(position.x, position.y);
	}

	// Pass by reference
	static public void SetAnchor(ref RectTransform rect, InterfaceAnchors anchor) {
		switch(anchor) {
			case InterfaceAnchors.BottomLeft:
				rect.anchorMax = Vector2.zero;
				rect.anchorMin = Vector2.zero;
				break;
			case InterfaceAnchors.Right:
				rect.anchorMax = new Vector2(1f, 0.5f);
				rect.anchorMin = new Vector2(1f, 0.5f);
				break;
			default:
				// Nothing implemented yet
				Debug.LogWarning("NOTHING IMPLEMENTED");
				break;
		}
	}
}
