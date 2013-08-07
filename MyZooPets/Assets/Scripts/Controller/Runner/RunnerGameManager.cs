using UnityEngine;
using System.Collections;

public class RunnerGameManager : MonoBehaviour {

    public bool GameRunning
    {
        get;
        protected set;
    }

	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
	
	}

    public void ActivateGameOver() {
        GameRunning = false;

        // Disable the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
        // 
    }

    void ResetGame() {
        GameRunning = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
       
        // Turn player on, if it isnt
        player.SetActive(true);
    }
}
