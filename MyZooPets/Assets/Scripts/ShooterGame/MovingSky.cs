using UnityEngine;
using System.Collections;

public class MovingSky : MonoBehaviour {
	public bool inSky;
	public Transform posSky;
	public bool isSun;
	// Use this for initialization
	void Start () {

		// then set the bool value based on where we are
		if (transform.position == posSky.position){
			inSky = true;
		}
		else{
			inSky = false;
		}
	}
}
