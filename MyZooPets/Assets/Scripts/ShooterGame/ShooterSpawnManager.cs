using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterSpawnManager :Singleton<ShooterSpawnManager>{
	//first spawner
	public GameObject firstPos;
	//second spawner
	public GameObject secondPos;
	// third spawner
	public GameObject thirdPos;
	//enemy prefab
	public GameObject enemyPrefab;
	public bool isSpawing;
	private float lastSpawn;
	public float spawnTime;
	//list of positions to spawn enemy from
	List<Vector3> posList;
	public List <EnemyData> enemy;
	public GameObject bulletPrefab;
	public GameObject powerUpPrefab;
			
	// Use this for initialization
	void Start(){
		posList = new List<Vector3>();
		posList.Add(firstPos.transform.position);
		posList.Add(secondPos.transform.position);	
		posList.Add(thirdPos.transform.position);
		ShooterGameManager.OnStateChanged += OnGameStateChanged;
	}
	// prevents finishing the last wave
	public void Reset(){
		StopCoroutine("SpawnEnemies");
	}

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			StopCoroutine("SpawnEnemies");
			break;
		case MinigameStates.Paused:
			isSpawing = false;
			break;
		case MinigameStates.Playing:
			isSpawing = true;
			break;
		case MinigameStates.Restarting:
			StopCoroutine("SpawnEnemies");
			break;
		}
	}

	public void SpawnTrigger(List<EnemyData> enemy){
		StartCoroutine("SpawnEnemies");
	}

	//Spawns all enemies in the list waiting 1 sec inbetween 
	IEnumerator SpawnEnemies(){
		for(int i = 0; i<enemy.Count; i++){
			if(isSpawing == false){
				yield return ShooterGameManager.Instance.Sync();
			}
			yield return new WaitForSeconds(1.0f);
			int rand = Random.Range(0, 3);
			//they are spawned in more of a weighted list fashion 
			//so while one of the first waves has only one hard enemy in it it can spawn more than one
			int RandomSpawn = Random.Range(0, enemy.Count);
			if(enemy[RandomSpawn].name == "powerUp"){
				Debug.Log(enemy[RandomSpawn].name);
				GameObject powerUp = Instantiate(powerUpPrefab, posList[rand], powerUpPrefab.transform.rotation) as GameObject;
				powerUp.GetComponent<ShooterPowerUpScript>().name = enemy[RandomSpawn].name;
				powerUp.GetComponent<ShooterPowerUpScript>().spriteName = enemy[RandomSpawn].spriteName;
				powerUp.GetComponent<ShooterPowerUpScript>().aiScript = enemy[RandomSpawn].aiScript;
				powerUp.GetComponent<ShooterPowerUpScript>().Initialize();
			}
			else{
				GameObject enemy1 = Instantiate(enemyPrefab, posList[rand], enemyPrefab.transform.rotation)as GameObject;
				enemy1.GetComponent<Enemy>().name = enemy[0].name;
				enemy1.GetComponent<Enemy>().spriteName = enemy[0].spriteName;
				enemy1.GetComponent<Enemy>().aiScript = enemy[RandomSpawn].aiScript;
				enemy1.GetComponent<Enemy>().bulletPrefab = bulletPrefab;
				enemy1.GetComponent<Enemy>().Initialize();
			}
		}
	}
}
