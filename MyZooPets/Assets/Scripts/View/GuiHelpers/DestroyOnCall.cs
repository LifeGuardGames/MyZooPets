using UnityEngine;
using System.Collections;

/// <summary>
/// Destroy on call.
/// Self destruct used for callbacks.
/// </summary>

public class DestroyOnCall : MonoBehaviour {
	public void DestroySelf(){
		Destroy(gameObject);
	}
}
