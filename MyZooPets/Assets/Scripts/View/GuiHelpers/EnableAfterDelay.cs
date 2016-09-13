using UnityEngine;
using System.Collections;

public class EnableAfterDelay : MonoBehaviour {
	public GameObject obj;
	public float delay = 0f;

	void Start() {
		StartCoroutine("StartAfterDelay");
	}

	public IEnumerator StartAfterDelay() {
		obj.SetActive(false);
		yield return new WaitForSeconds(delay);
		obj.SetActive(true);
	}
}
