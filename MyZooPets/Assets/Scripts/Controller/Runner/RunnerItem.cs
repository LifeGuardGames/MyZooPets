using UnityEngine;
using System.Collections;

public abstract class RunnerItem : MonoBehaviour {
	public string Name = "";
	public int PointValue = 0;

	// Use this for initialization
	public virtual void Start() {
	}
	
	// Update is called once per frame
	public virtual void Update() {
		
	}

	void OnTriggerEnter(Collider inOther) {
		if (inOther.gameObject.tag == "Player") {
			OnPickup();

            ScoreManager scoreManager = RunnerGameManager.GetInstance().ScoreManager;
            scoreManager.AddPoints(PointValue);
		}
	}

	// Define what you want the item to do on pickup here
	public abstract void OnPickup();
}
