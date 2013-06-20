using UnityEngine;
using System.Collections;

public class Highlighting : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.touchCount > 0){
			Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(myRay,out hit)){
				transform.localScale = new Vector3 (2,2,2);
			}
			else{
				transform.localScale = new Vector3(2,2,2);
			}
		}
	}
}
