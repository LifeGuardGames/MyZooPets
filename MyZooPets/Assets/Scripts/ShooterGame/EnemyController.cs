using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : Singleton<EnemyController> {

	public List<EnemyData> EnemyList;
	public int EnemiesInWave=0;
	Wave waver;
	Wave CurrWave;
	public GameObject EnemyPrefab;

	public void reset(){
		EnemiesInWave=0;
		if(EnemyList!=null){
		EnemyList.Clear();
		}
		EnemyList = new List<EnemyData>();
		BuildEnemyList(DataLoaderTriggerArmy.GetDataList());

	}

	// builds a list of waves
	public Wave buildWave (int _WaveNum){

		int difficulty=0;
		if(_WaveNum<3){
			difficulty=1;
		}
		else if (_WaveNum >=3 &&_WaveNum<6){
			difficulty=2;
		}
		else{
			difficulty=3;
		}
		waver = new Wave();
	
		ImmutableDataWave LoadedWave = DataLoaderWaves.GetWave(difficulty);
		waver.NumOfEnemies= LoadedWave.NumOfEnemies;
		waver.NumOfBasics=LoadedWave.BegEnemies;
		waver.NumOfMedium=LoadedWave.MediumEnemies;
		waver.NumOfHard=LoadedWave.HardEnemies;
		return waver;
	}

	// builds a list of enemy types
	public void BuildEnemyList(List<ImmutableDataTriggerArmy> mobList){
		foreach (ImmutableDataTriggerArmy baddie in mobList){
			EnemyData mob = new EnemyData();
			mob.name = baddie.Name;
			mob.spritz= baddie.SpriteName;
			mob.AiScript= baddie.AI;
			EnemyList.Add(mob);

		}
		GenerateWave(0);
	}
	// determines which wave to spawn and set the enemies in wave to the correct amount
	public void GenerateWave(int _WaveNum){
		CurrWave=buildWave(_WaveNum);
		EnemiesInWave= int.Parse(CurrWave.NumOfEnemies);
		SpawnWave(CurrWave);
	}

	// Spawns the current wave
	public void SpawnWave(Wave currWave){

		List<EnemyData> WaveEnemies;
		WaveEnemies = new List<EnemyData>();
		for (int i =0; i < EnemiesInWave; i++){
			WaveEnemies.Add(EnemyList[0]);
		}
		/*for (int i =0; i < currWave.NumOfBasics; i++){
			WaveEnemies.Add(EnemyList[0]);
		}
		for (int i =0; i < currWave.NumOfMedium; i++){
			WaveEnemies.Add(EnemyList[1]);
		}
		for (int i =0; i < currWave.NumOfHard; i++){
			WaveEnemies.Add(EnemyList[2]);
		}*/
		SpawnManager.Instance.EnemySpawnCount=EnemiesInWave;
		SpawnManager.Instance.IsSpawing=true;

	}

	// checks if all enemies are dead and if they are 
	public void CheckEnemiesInWave(){
		if (EnemiesInWave == 0){
			Debug.Log("check enemies in wave");
			InhalerManager.Instance.CanUseInhalerButton=!InhalerManager.Instance.CanUseInhalerButton;
			ShooterUIManager.Instance.AChangeOfTimeActOne();
		}
	}
}
