using UnityEngine;
using System.Collections;

public class ShooterGameManager : MinigameManager<ShooterGameManager> {

	//used to calculate fire rate
	private float lastFire = 0.0f;
	// used to calculate spawn rate
	private float lastSpawn = 0.0f;
	//Rate of Fire
	public float timeBetweenFire = 1.0f;
	//Fireball scale
	public float fBallScale=1.0f;
	//Rate of enemy spawn at a spawner
	public float spawnTimer = 2.0f; 
	//The Spawn Manager 
	GameObject Spawner;
	GameObject bullet;
	public GameObject Player;
	//player health
	public float playerHealth;

	void Awake(){
		Application.targetFrameRate = 60;
		quitGameScene = SceneUtils.BEDROOM;
	}
	protected override void _Start (){}
	protected override void _OnDestroy(){
		Application.targetFrameRate = 30;
	}

	protected override string GetMinigameKey ()
	{
		return "ShooterGame";
	}
	protected override bool IsTutorialOn(){
		return false;	//TODO Change
	}
	
	protected override bool HasCutscene(){
		return false;	//TODO Change
	}

	protected override void _NewGame ()
	{
		Debug.Log ("hi");
		lastFire= Time.time;
		lastSpawn= Time.time;
		Spawner= GameObject.Find ("Spawner");
	}
	public override int GetReward (MinigameRewardTypes eType)
	{
		return GetStandardReward(eType);
	}

	public void spawnEnemy()
	{
		if (spawnTimer-lastSpawn<0)
		{
			int randomeSpawner = Random.Range(0,3);
			Spawner.GetComponent<SpawnManager>().spawnTrigger(randomeSpawner);
		}
	}
	void OnTap(TapGesture gestures)
	{
		Debug.Log("SDFD");
		//shoot(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
	}
	public void shoot(Vector3 dir)
	{
		GameObject instance = Instantiate(bullet,Player.transform.position,bullet.transform.rotation)as GameObject;
		instance.transform.Rotate(dir);
	}
	
	// Update is called once per frame
	protected override void _Update() {

	}
	public void removeHealth(float amount)
	{
		playerHealth+=amount;
		timeBetweenFire+=amount;
		fBallScale-=amount;
	}
}
