using UnityEngine;
using System.Collections;

public class MovingSky : MonoBehaviour {
	public bool InSky;
	public GameObject PosSky;
	// Use this for initialization
	void Start () {
	if (transform.position == PosSky.transform.position){
			InSky=true;
		}
		else{
			InSky=false;
		}
	}
}
