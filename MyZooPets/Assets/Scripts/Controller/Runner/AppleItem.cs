using UnityEngine;
using System.Collections;

public class AppleItem : RunnerItem
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public override void OnPickup()
    {
        Debug.Log("Hello!");
        GameObject.Destroy(gameObject);
    }
}
