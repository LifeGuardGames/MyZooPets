using UnityEngine;
using System.Collections;

public class ButtonResetGame : MonoBehaviour{

	// Use this for initialization
	void Start(){
	
	}
	
	// Update is called once per frame
	void Update(){
	
	}

	void OnClick(){
		Debug.LogWarning("This should not be called");
		RunnerGameManager.Instance.NewGame();
	}
}
