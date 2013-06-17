using UnityEngine;
using System.Collections;

public class buttonArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (InhalerLogic.CurrentStep == 2 && InhalerLogic.CurrentInhalerType == InhalerType.Advair){
            renderer.enabled = true;
        }	
		else
		{
			renderer.enabled = false;
		}
	}
}
