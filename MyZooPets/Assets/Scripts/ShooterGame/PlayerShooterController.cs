using UnityEngine;
using System.Collections;

public class PlayerShooterController : Singleton<PlayerShooterController> {
	//the player state this dictates the pets strength
	public string State = "neutral";
	//player health
	public float PlayerHealth;
	// the fireball scale
	public float FBallScale;
	//The bar Manager used to retrieve health data from bar usage

	// our score
	public int Score=0;
	public GameObject ScoreLabel;
	public GameObject bulletSpawn;
	public GameObject bullet;
	// Use this for initialization
	void Start () {
		ScoreLabel.GetComponent<UILabel>().text = Score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	}
	private void ChangeState(string _state){
		State = _state;
		switch(State){
		case "happy":{
			FBallScale=1.5f;
			break;
			}
		case "neutral":{
			FBallScale=1.0f;
			break;
			}
		case "distressed":{
			FBallScale=0.5f;
			break;
			}
		}
	}
	public void removeHealth (float amount){
		PlayerHealth += amount;
		if (PlayerHealth == 11){
			ChangeState("happy");
		}
		else if (PlayerHealth >5 && PlayerHealth <=10){
			ChangeState("neutral");
		}
		else if (PlayerHealth<=5){
			ChangeState("distressed");
		}
	}
	public void AddScore(int amount)
	{
		Score+=amount;
		ScoreLabel.GetComponent<UILabel>().text = Score.ToString();
	}
	public void shoot(Vector3 dir){
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);
		//fBallScale = Player.GetComponent<Player>().FBallScale;
		GameObject instance = Instantiate(bullet, bulletSpawn.transform.position, bullet.transform.rotation)as GameObject;
		instance.GetComponent<bulletScript>().target = lookPos;
		instance.GetComponent<bulletScript>().FindTarget();
		instance.gameObject.transform.localScale /= FBallScale;
	}
}
