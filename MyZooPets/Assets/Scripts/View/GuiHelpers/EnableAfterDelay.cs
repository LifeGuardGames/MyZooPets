using UnityEngine;
using System.Collections;

public class EnableAfterDelay : MonoBehaviour {
	
	public GameObject obj;
	public float delay = 0f;
	
	// Use this for initialization
	void Start () {
		 StartCoroutine("StartAfterDelay");
	}
	
	public IEnumerator StartAfterDelay(){
		yield return new WaitForSeconds(delay);
		obj.SetActive(true);
	}
}
