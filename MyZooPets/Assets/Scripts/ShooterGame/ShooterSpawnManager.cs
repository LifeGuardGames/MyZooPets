using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterSpawnManager :Singleton<ShooterSpawnManager>{
	public GameObject enemyPrefab;		//enemy prefab
	public bool isSpawing;
	private float lastSpawn;
	public float spawnTime;

	public List<GameObject> posList;	//list of positions to spawn enemy from
	private List<ImmutableDataShooterArmy> spawningList;
			
	// Use this for initialization
	void Start(){
		ShooterGameManager.OnStateChanged += OnGameStateChanged;
	}

	// prevents finishing the last wave
	public void Reset(){
		StopCoroutine("SpawnEnemies");
		spawningList = null;

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
		}
	}

	public void Quit(){
		StopCoroutine("SpawnEnemies");
	}

	public void SpawnTriggers(List<ImmutableDataShooterArmy> listToSpawn){
		spawningList = listToSpawn;
		StartCoroutine("SpawnEnemies");
	}
	public void SpawnBoss(){
		int rand = Random.Range(0,2);
		if(rand == 0){
			GameObject spawnPrefab = Resources.Load("ShooterEnemyBoss") as GameObject;
			GameObjectUtils.AddChild(posList[0], spawnPrefab, isPreserveLayer:true);
		}
		else{
			GameObject spawnPrefab = Resources.Load("ShooterEnemyBossWaller") as GameObject;
			GameObjectUtils.AddChild(posList[0], spawnPrefab, isPreserveLayer:true);
		}
	}

	public void SpawnPowerUp(){
		int randPowerUp = Random.Range(5,7);
		GameObject spawnPrefab = Resources.Load(DataLoaderShooterArmy.GetData("Mober_" + randPowerUp.ToString()).PrefabName) as GameObject;
		int randomPositionIndex = Random.Range(0, 3);
		GameObjectUtils.AddChild(posList[randomPositionIndex], spawnPrefab, isPreserveLayer:true);
	}

	//Spawns all enemies in the list waiting 1 sec inbetween 
	IEnumerator SpawnEnemies(){
		for(int i = 0; i < spawningList.Count; i++){
			if(isSpawing == false){
				yield return ShooterGameManager.Instance.Sync();
			}
			yield return new WaitForSeconds(1.0f);
			int randomPositionIndex = Random.Range(0, 3);
			
			//they are spawned in more of a weighted list fashion 
			//so while one of the first waves has only one hard enemy in it it can spawn more than one
			int randomSpawnIndex = Random.Range(0, spawningList.Count);
			if(spawningList[randomSpawnIndex].Id != "Mober_4"){
			GameObject spawnPrefab = Resources.Load(spawningList[randomSpawnIndex].PrefabName) as GameObject;
			GameObjectUtils.AddChild(posList[randomPositionIndex], spawnPrefab, isPreserveLayer:true);
			}
			else{
				randomPositionIndex = Random.Range(3,5);
				GameObject spawnPrefab = Resources.Load("ShooterEnemySeeker") as GameObject;
				GameObjectUtils.AddChild(posList[randomPositionIndex], spawnPrefab, isPreserveLayer:true);
			}
		}
	}
}
