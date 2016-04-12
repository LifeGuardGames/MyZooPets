using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShooterGameEnemyController : Singleton<ShooterGameEnemyController>{
	public EventHandler<EventArgs> OnTutorialStepDone;					//tutorial event handeler
	public List<ImmutableDataShooterArmy> enemyList;		// list of the various enemy types
	public int enemiesInWave = 0;							// the number of enemies in the current wave											
	private ImmutableDataWave currentWave;					// our current wave

	public void Reset(){
		enemiesInWave = 0;
		if(enemyList != null){
			enemyList.Clear();
		}
		currentWave = null;
		BuildEnemyList();
	}

	// builds a list of waves
	public ImmutableDataWave buildWave(int waveNumber){
		int difficulty = 0;
		if(waveNumber == 1){
			return DataLoaderWaves.GetData("Boss Wave_1");
		}
		if(waveNumber == 0){
			difficulty = 0;
		}
		else if(waveNumber > 0 && waveNumber < 4){
			difficulty = 1;
		}
		else if(waveNumber >= 4 && waveNumber < 7){
			difficulty = 2;
		}
		else{
			difficulty = 3;
		}
		return DataLoaderWaves.GetWave(difficulty);
	}

	// builds a list of enemy types
	public void BuildEnemyList(){
		enemyList = DataLoaderShooterArmy.GetDataList();

		// if we are in tutorial generate the test wave
		if(ShooterGameManager.Instance.inTutorial == true){
			GenerateWave(0);
		}
		else{
			GenerateWave(1);
		}
	}

	// determines which wave to spawn and set the enemies in wave to the correct amount
	public void GenerateWave(int waveNumber){
		currentWave = buildWave(waveNumber);
		enemiesInWave = currentWave.TotalEnemies;
		SpawnWave(currentWave);
	}

	// Spawns the current wave
	public void SpawnWave(ImmutableDataWave currWave){
		// build an array of varying enemy types that array is then passed to the spawner manager
		List<ImmutableDataShooterArmy> waveEnemies = new List<ImmutableDataShooterArmy>();

		for(int i = 0; i < currWave.BegEnemiesCount; i++){
			waveEnemies.Add(enemyList[0]);
		}
		for(int i = 0; i < currWave.MediumEnemiesCount; i++){
			waveEnemies.Add(enemyList[1]);
		}
		for(int i = 0; i < currWave.HardEnemiesCount; i++){
			waveEnemies.Add(enemyList[2]);
		}
		for(int i = 0; i < currWave.PowerUpCount; i++){
			int rand = UnityEngine.Random.Range(3, 5);
			waveEnemies.Add(enemyList[rand]);
		}
		if(currWave.BossRound == true){
			ShooterSpawnManager.Instance.SpawnBoss();
		}
		ShooterSpawnManager.Instance.SpawnTriggers(waveEnemies);
	}

	// checks if all enemies are dead and if yes, begin day transition
	public void CheckEnemiesInWave(){
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.Playing){
			if(enemiesInWave == 0){
				if(ShooterGameManager.Instance.inTutorial){
					if(OnTutorialStepDone != null){
						OnTutorialStepDone(this, EventArgs.Empty);
					}
				}
				ShooterInhalerManager.Instance.CanUseInhalerButton = false;
				ShooterGameManager.Instance.StartTimeTransition();
			}
		}
	}
}
