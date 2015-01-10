using UnityEngine;
using System.Collections;

public class PlayerShooterController : MonoBehaviour {

	//player health
	public float PlayerHealth;
	// the fireball scale
	public float FBallScale;
	//The bar Manager used to retrieve health data from bar usage
	public GameObject BManager;
	// our score
	public int Score=0;
	public GameObject ScoreLabel;
	public GameObject bullet;
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
	/*public void shoot(Vector3 dir){
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(dir);
		//fBallScale = Player.GetComponent<Player>().FBallScale;
		GameObject instance = Instantiate(bullet, bulletSpawn.transform.position, bullet.transform.rotation)as GameObject;
		instance.gameObject.transform.localScale /= fBallScale;
		instance.GetComponent<bulletScript>().target = dir;
	}*/
}
