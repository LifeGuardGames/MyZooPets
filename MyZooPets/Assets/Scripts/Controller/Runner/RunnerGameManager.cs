using UnityEngine;
using System.Collections;

public class RunnerGameManager : MonoBehaviour {
    private PlayerRunner mPlayerRunner;

    public bool GameRunning
    {
        get;
        protected set;
    }

	// Use this for initialization
	void Start() {
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
            mPlayerRunner = playerObject.GetComponent<PlayerRunner>();
	}
	
	// Update is called once per frame
	void Update() {
	}

    public void ActivateGameOver() {
        GameRunning = false;

        // Disable the player
        if (mPlayerRunner != null)
            mPlayerRunner.gameObject.SetActive(false);
    }

    void ResetGame() {
        GameRunning = true;

        // Turn player on, if it isnt
        if (mPlayerRunner != null)
            mPlayerRunner.gameObject.SetActive(true);
    }
}
