using UnityEngine;
using System.Collections;

public class CoinItem : RunnerItem {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnPickup()
    {
        Debug.Log("Coin Picked Up!!");
        GameObject.Destroy(gameObject);
    }
}
