using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShooterGameEnemyController : Singleton<ShooterGameEnemyController> {
	public EventHandler<EventArgs> proceed;					//tutorial event handeler
	public List<EnemyData> enemyList;						// list of the various enemy types
	public int enemiesInWave = 0;							// the number of enemies in the current wave											
	Wave currWave;											// our current wave

	public void reset(){
		enemiesInWave = 0;
		if(enemyList!=null){
		enemyList.Clear();
		}

		BuildEnemyList();
	}

	// builds a list of waves
	public Wave buildWave (int _WaveNum){
		Wave waver;	
		int difficulty = 0;
		if(_WaveNum==0){
			difficulty = 0;
		}
		else if(_WaveNum>0&&_WaveNum<4){
			difficulty = 1;
		}
		else if (_WaveNum >=4 &&_WaveNum<7){
			difficulty = 2;
		}
		else{
			difficulty = 3;
		}
		waver = new Wave();
	
		ImmutableDataWave LoadedWave = DataLoaderWaves.GetWave(difficulty);
		waver.numOfEnemies = LoadedWave.NumOfEnemies;
		waver.numOfBasics = LoadedWave.BegEnemies;
		waver.numOfMedium = LoadedWave.MediumEnemies;
		waver.numOfHard = LoadedWave.HardEnemies;
		waver.numOfPowerUps = LoadedWave.PowerUp;
		return waver;
	}

	// builds a list of enemy types
	public void BuildEnemyList(){
			enemyList = new List<EnemyData>();
		for(int i = 0; i < 5; i++){
			EnemyData mob = new EnemyData();
			ImmutableDataTriggerArmy baddie = DataLoaderTriggerArmy.GetEnemy(i);
			mob.name = baddie.Name;
			mob.spriteName= baddie.SpriteName;
			mob.aiScript= baddie.AI;
			enemyList.Add(mob);

		}
		// if we are in tutorial generate the test wave
		if(ShooterGameManager.Instance.inTutorial == true){
			GenerateWave(0);
		}
		else{
		GenerateWave(1);
		}
	}
	// determines which wave to spawn and set the enemies in wave to the correct amount
	public void GenerateWave(int _WaveNum){
		currWave=buildWave(_WaveNum);
		enemiesInWave= int.Parse(currWave.numOfEnemies);
		SpawnWave(currWave);
	}

	// Spawns the current wave
	public void SpawnWave(Wave currWave){
		// here wed basically build an array of varying enemy types that array is then passed to the spawner manager
		List<EnemyData> WaveEnemies;
		WaveEnemies = new List<EnemyData>();
		/*for (int i = 0; i < EnemiesInWave; i++){
			WaveEnemies.Add(EnemyList[0]);
		}*/
		for (int i =0; i < int.Parse(currWave.numOfBasics); i++){
			WaveEnemies.Add(enemyList[0]);
			Debug.Log(enemyList[0].name);
		}
		for (int i =0; i < int.Parse(currWave.numOfMedium); i++){
			WaveEnemies.Add(enemyList[1]);
		}
		for (int i =0; i < int.Parse(currWave.numOfHard); i++){
			WaveEnemies.Add(enemyList[2]);
		}
		for (int i =0; i < int.Parse(currWave.numOfPowerUps); i++){
			Debug.Log(currWave.numOfPowerUps);
			int rand = UnityEngine.Random.Range(3,5);
			WaveEnemies.Add(enemyList[rand]);
		}
		//ShooterSpawnManager.Instance.EnemySpawnCount=EnemiesInWave;
		ShooterSpawnManager.Instance.enemy = WaveEnemies;
		ShooterSpawnManager.Instance.spawnTrigger(WaveEnemies);

	}

	// checks if all enemies are dead and if they are beings day transition
	public void CheckEnemiesInWave(){
		if (enemiesInWave == 0){
			if(ShooterGameManager.Instance.inTutorial){
				if(proceed != null)
					proceed(this, EventArgs.Empty);
			}
			ShooterInhalerManager.Instance.CanUseInhalerButton=false;
			ShooterUIManager.Instance.AChangeOfTimeActOne();
		}
	}
}
