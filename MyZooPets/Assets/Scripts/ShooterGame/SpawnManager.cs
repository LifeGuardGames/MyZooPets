using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager :MonoBehaviour {
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

	static public bool IsActive{
		get{
			return _instance !=null;
		}
	}
	static public SpawnManager instance{
		get{
			if(_instance==null){
				_instance = Object.FindObjectOfType(typeof(SpawnManager))as SpawnManager;
				if(_instance == null){
					GameObject thing = new GameObject ("SpawnManager");
					_instance=thing.AddComponent<SpawnManager>();
				}
			}
			return _instance;

		}
	}
			
	// Use this for initialization
	void Start () {
		posList = new List<Vector3>();
		posList.Add(firstPos.transform.position);
		posList.Add (secondPos.transform.position);	
		posList.Add(thirdPos.transform.position);

	}
	
	public void spawnTrigger(Enemy[] enemy){
		for ( int i =0; i <enemy.Length;i++){
		int rand = Random.Range(0,3);
		GameObject enemy1 = Instantiate(enemyPrefab,posList[rand],enemyPrefab.transform.rotation)as GameObject;
		enemy1.GetComponent<Enemy>().Initialize(enemy[i]);
		}
	}

}
