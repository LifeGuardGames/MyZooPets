using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//player health
	public float playerHealth;
	public float fBallScale;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void removeHealth (float amount){
		playerHealth += amount;
		//timeBetweenFire += amount;
		fBallScale -= amount;
	}
}
