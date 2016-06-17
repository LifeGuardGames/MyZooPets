using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterSpawnManager :Singleton<ShooterSpawnManager>{
	public GameObject enemyPrefab;		//enemy prefab
	private float lastSpawn;
	public float spawnTime;

	public List<GameObject> posList;	//list of positions to spawn enemy from
	private List<ImmutableDataShooterArmy> spawningList;
			

	// prevents finishing the last wave
	public void Reset(){
		StopAllCoroutines();
		spawningList = null;
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
		int randPowerUp = Random.Range(5,8);
		Debug.Log(DataLoaderShooterArmy.GetData("Mober_" + 7.ToString()).PrefabName);
		GameObject spawnPrefab = Resources.Load(DataLoaderShooterArmy.GetData("Mober_" + 7.ToString()).PrefabName) as GameObject;
		int randomPositionIndex = Random.Range(0, 3);
		Debug.Log(spawnPrefab.name);
		GameObjectUtils.AddChild(posList[randomPositionIndex], spawnPrefab, isPreserveLayer:true);
	}

	//Spawns all enemies in the list waiting 1 sec inbetween 
	IEnumerator SpawnEnemies() {
		yield return new WaitForSeconds(1.0f);
		if(ShooterGameEnemyController.Instance.enemiesInWave > 0) {
			if(!ShooterGameManager.Instance.isPaused && !ShooterGameManager.Instance.isGameOver) {
				int randomPositionIndex = Random.Range(0, 3);

				//they are spawned in more of a weighted list fashion 
				//so while one of the first waves has only one hard enemy in it it can spawn more than one
				int randomSpawnIndex = Random.Range(0, spawningList.Count);
				if(spawningList[randomSpawnIndex].Id != "Mober_4") {
					GameObject spawnPrefab = Resources.Load(spawningList[randomSpawnIndex].PrefabName) as GameObject;
					GameObjectUtils.AddChild(posList[randomPositionIndex], spawnPrefab, isPreserveLayer: true);
				}
				else {
					randomPositionIndex = Random.Range(3, 5);
					GameObject spawnPrefab = Resources.Load("ShooterEnemySeeker") as GameObject;
					GameObjectUtils.AddChild(posList[randomPositionIndex], spawnPrefab, isPreserveLayer: true);
				}
				StartCoroutine("SpawnEnemies");
			}
			else {
				StartCoroutine("WaitASec");
			}
		}
	}
	

	IEnumerator WaitASec() {
		yield return new WaitForSeconds(0.1f);
		if(ShooterGameManager.Instance.isPaused) {
			StartCoroutine(WaitASec());
		}
		else {
			StartCoroutine("SpawnEnemies");
		}
    }
}
