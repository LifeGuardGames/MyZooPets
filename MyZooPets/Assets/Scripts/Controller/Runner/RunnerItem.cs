using UnityEngine;
using System.Collections;

public abstract class RunnerItem : MonoBehaviour {
	public string Name = "";
	public int PointValue = 0;

	protected ScoreManager mScoreManager;

	// Use this for initialization
	public virtual void Start() {
		GameObject scoreManagerObject = GameObject.Find("ScoreManager");
		if (scoreManagerObject != null) {
				mScoreManager = scoreManagerObject.GetComponent<ScoreManager>();
		}
	}
	
	// Update is called once per frame
	public virtual void Update() {
		
	}

	void OnTriggerEnter(Collider inOther) {
		if (inOther.gameObject.tag == "Player") {
			Debug.Log("Picking up " + Name + ". Adding point value " + PointValue);
			OnPickup();

			if (mScoreManager != null)
				mScoreManager.AddPoints(PointValue);
		}
	}

	// Define what you want the item to do on pickup here
	public abstract void OnPickup();
}
