using UnityEngine;
using System.Collections;

public class ClickTest : MonoBehaviour {

	void OnMouseDown(){
		Debug.Log("CLICKED");
	}

	void OnTap(TapGesture e){
		Debug.Log("sdfsdf");
	}

	public void Print(){
		Debug.Log("Print call");
	}
}
