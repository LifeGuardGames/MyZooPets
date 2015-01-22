using UnityEngine;
using System.Collections;

public class MovingSky : MonoBehaviour {
	public bool InSky;
	public GameObject PosSky;
	public bool isSun;
	// Use this for initialization
	void Start () {
		transform.position = PosSky.transform.position;
		if (transform.position == PosSky.transform.position){
			InSky = true;
		}
		else{
			InSky=false;
		}
	}
}
