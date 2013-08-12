using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    public UILabel ScoreLabel = null;

    private int mPlayerPoints = 0;

	// Use this for initialization
    void Start()
    {
        AddPoints(0);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void AddPoints(int inNumPointsToAdd) {
        mPlayerPoints += inNumPointsToAdd;
        Debug.Log("New score: " + mPlayerPoints);
        if (ScoreLabel != null)
        {
            ScoreLabel.text = "Score: " + mPlayerPoints;
        }
    }
}
