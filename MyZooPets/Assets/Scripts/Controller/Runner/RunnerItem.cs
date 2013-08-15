using UnityEngine;
using System.Collections;

public abstract class RunnerItem : MonoBehaviour
{
	public string Name = "";
	public int PointValue = 0;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider inOther)
	{
		if (inOther.gameObject.tag == "Player")
		{
			Debug.Log("Picking up " + Name + ". Adding point value " + PointValue);
			OnPickup();

            GameObject scoreManagerObject = GameObject.Find("ScoreManager");
            if (scoreManagerObject != null) {
                ScoreManager scoreManager = (ScoreManager)scoreManagerObject.GetComponent<ScoreManager>();
                if (scoreManager != null) {
                    scoreManager.AddPoints(PointValue);
                }
            }
		}
	}

	// Define what you want the item to do on pickup here
	public abstract void OnPickup();
}
