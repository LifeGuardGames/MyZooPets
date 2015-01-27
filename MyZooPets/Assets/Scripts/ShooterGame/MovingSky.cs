using UnityEngine;
using System.Collections;

public class MovingSky : MonoBehaviour {
	public bool InSky;
	public Transform PosSky;
	public bool isSun;
	// Use this for initialization
	void Start () {
		transform.position = PosSky.position;
		if (transform.position == PosSky.position){
			InSky = true;
		}
		else{
			InSky=false;
		}
	}
}
