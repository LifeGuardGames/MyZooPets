using UnityEngine;
using System.Collections;

public class ButtonResetGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick() {
        RunnerGameManager gameManager = RunnerGameManager.GetInstance();
        gameManager.ResetGame();
	}
}
