﻿using UnityEngine;
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
	public bool IsSpawing;
	private float LastSpawn;
	public float SpawnTime;
	//list of positions to spawn enemy from
	List<Vector3> posList;
	public List <EnemyData> enemy;
			
	// Use this for initialization
	void Start () {
		posList = new List<Vector3>();
		posList.Add(firstPos.transform.position);
		posList.Add (secondPos.transform.position);	
		posList.Add(thirdPos.transform.position);
		LastSpawn=Time.time;
		ShooterGameManager.OnStateChanged+= OnGameStateChanged;
	}

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			IsSpawing=false;
			break;
		case MinigameStates.Paused:
			IsSpawing=false;
			break;
		case MinigameStates.Playing:
			IsSpawing=true;
			break;
		}
	}
	public void spawnTrigger(){
		//Debug.Log(enemy.Count);
		enemy = EnemyController.Instance.EnemyList;
		int rand = Random.Range(0,3);
		GameObject enemy1 = Instantiate(enemyPrefab,posList[rand],enemyPrefab.transform.rotation)as GameObject;
		enemy1.GetComponent<Enemy>().name = enemy[0].name;
		enemy1.GetComponent<Enemy>().spritz = enemy[0].spritz;
		enemy1.GetComponent<Enemy>().AiScript = enemy[0].AiScript;
		enemy1.GetComponent<Enemy>().Initialize();
	}
	void Update(){
		if(IsSpawing ==true){
			if(LastSpawn<= Time.time-SpawnTime){
				spawnTrigger();
				LastSpawn=Time.time;
			}
		}
		if(EnemyController.Instance.EnemiesInWave==0){
			IsSpawing=false;
		}
	}
}
