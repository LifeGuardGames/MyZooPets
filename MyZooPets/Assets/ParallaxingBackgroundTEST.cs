using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxingBackgroundTEST : MonoBehaviour {
    public float ScrollSpeed = 0.05f;
    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
		float currentSpeed = ScrollSpeed * Time.time;
        renderer.material.mainTextureOffset = new Vector2(currentSpeed, 0f);
    }
}
