using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//player health
	public float PlayerHealth;
	// the fireball scale
	public float FBallScale;
	//The bar Manager used to retrieve health data from bar usage
	public GameObject BManager;
	// our score
	public int Score=0;
	public GameObject ScoreLabel;
	// Use this for initialization
	void Start () {
		ScoreLabel.GetComponent<UILabel>().text = Score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void removeHealth (float amount){
		PlayerHealth += amount;
		//timeBetweenFire += amount;
		FBallScale -= (amount/10);
		BManager.GetComponent<BarManager>().numMissed++;
	}
	public void AddScore(int amount)
	{
		Score+=amount;
		ScoreLabel.GetComponent<UILabel>().text = Score.ToString();
	}
}
