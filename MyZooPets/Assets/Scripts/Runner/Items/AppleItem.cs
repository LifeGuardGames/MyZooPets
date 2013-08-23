using UnityEngine;
using System.Collections;

public class AppleItem : RunnerItem {

	// Use this for initialization
	public override void Start() {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update() {
        base.Update();
	}

    public override void OnPickup() {
        GameObject.Destroy(gameObject);
    }
}
