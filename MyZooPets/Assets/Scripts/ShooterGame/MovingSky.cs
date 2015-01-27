using UnityEngine;
using System.Collections;

public class MovingSky : MonoBehaviour {
	public bool inSky;
	public Transform posSky;
	public bool isSun;
	// Use this for initialization
	void Start () {
		transform.position = posSky.position;
		if (transform.position == posSky.position){
			inSky = true;
		}
		else{
			inSky = false;
		}
	}
}
