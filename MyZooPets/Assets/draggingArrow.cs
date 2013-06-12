using UnityEngine;
using System.Collections;

public class draggingArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (InhalerLogic.CurrentStep == 4){
            renderer.enabled = true;
        }	
		else
		{
			renderer.enabled = false;
		}
	}
}
