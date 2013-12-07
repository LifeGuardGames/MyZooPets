using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor create empty in parent.
/// Editor Script: Creates an empty child in the selected parent with default transform and inherit parent layer
/// </summary>
public class EditorCreateEmptyInParent : MonoBehaviour {
	
	[MenuItem ("GameObject/Create Empty in Parent %#&c")]
	static void CreateEmptyInParent(){
		GameObject parent = Selection.activeTransform.gameObject;
		GameObject go = new GameObject();
		Transform t = go.transform;
		t.parent = parent.transform;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
		go.layer = parent.layer;
	}
	
	[MenuItem ("GameObject/Create Empty in Parent %#&c", true)]
	static bool ValidateCreateEmptyInParent(){
		return Selection.activeTransform != null;
	}
}
