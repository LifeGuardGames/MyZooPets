using UnityEngine;
using System.Collections;

public class CameraSortMode : MonoBehaviour {
	
	public TransparencySortMode sortMode;
	
	void Start () {
		gameObject.GetComponent<Camera>().transparencySortMode = sortMode;
	}
}
