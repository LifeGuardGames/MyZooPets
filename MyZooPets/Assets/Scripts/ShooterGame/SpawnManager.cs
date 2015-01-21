using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager :Singleton<SpawnManager>{
	static SpawnManager _instance;
	//first spawner
	public GameObject firstPos;
	//second spawner
	public GameObject secondPos;
	// third spawner
	public GameObject thirdPos;
	//enemy prefab
	public GameObject enemyPrefab;
	//list of positions to spawn enemy from
	List<Vector3> posList;
			
	// Use this for initialization
	void Start () {
		posList = new List<Vector3>();
		posList.Add(firstPos.transform.position);
		posList.Add (secondPos.transform.position);	
		posList.Add(thirdPos.transform.position);
	}

	public void spawnTrigger(List<EnemyData> enemy){
		//Debug.Log(enemy.Count);

			StartCoroutine("SpawnEnemy",enemy);

		
	}

	IEnumerator SpawnEnemy(List<EnemyData> enemy){
		for ( int i =0; i <enemy.Count;i++){
		yield return new WaitForSeconds(1.0f);
		int rand = Random.Range(0,3);
		GameObject enemy1 = Instantiate(enemyPrefab,posList[rand],enemyPrefab.transform.rotation)as GameObject;
		enemy1.GetComponent<Enemy>().name = enemy[0].name;
		enemy1.GetComponent<Enemy>().spritz = enemy[0].spritz;
		enemy1.GetComponent<Enemy>().AiScript = enemy[0].AiScript;
		enemy1.GetComponent<Enemy>().Initialize();
		}
	}

}
