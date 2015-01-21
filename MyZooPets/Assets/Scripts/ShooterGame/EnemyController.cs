using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

	public List<EnemyData> EnemyList;
	public List <Wave> waves;
	public int EnemiesInWave=0;

	Wave waver;
	Wave CurrWave;
	public GameObject EnemyPrefab;

	public void reset(){
		if(EnemyList!=null){
		EnemyList.Clear();
		}
		EnemyList = new List<EnemyData>();
		waves = new List<Wave>();
		BuildEnemyList(DataLoaderTriggerArmy.GetDataList());
	}
	// builds a list of waves
	public void buildWaveList (List<ImmutableDataWaves> waveList){
		foreach(ImmutableDataWaves tsunami in waveList){
			waver = new Wave();
			Debug.Log(tsunami.NumOfEnemies);
			waver.NumOfEnemies = tsunami.NumOfEnemies;
			waver.NumOfBasics= tsunami.BegEnemies;
			waver.NumOfMedium = tsunami.MediumEnemies;
			waver.NumOfHard= tsunami.HardEnemies;
			waves.Add(waver);
		}
		GenerateWave(0);
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
		buildWaveList(DataLoaderWaves.GetDataList());
	}
	// determines which wave to spawn and set the enemies in wave to the correct amount
	public void GenerateWave(int _WaveNum){
		Debug.Log(_WaveNum);
		CurrWave=waves[_WaveNum];
		EnemiesInWave= int.Parse(waves[_WaveNum].NumOfEnemies);
		SpawnWave(CurrWave);
	}
	// Spawns the current wave
	public void SpawnWave(Wave currWave){
		List<EnemyData> WaveEnemies;
		WaveEnemies = new List<EnemyData>();
		for (int i =0; i < EnemiesInWave; i++){
			WaveEnemies.Add(EnemyList[0]);
		}
		SpawnManager.Instance.spawnTrigger(WaveEnemies);

	}
	// checks if all enemies are dead and if they are 
	public void CheckEnemiesInWave(){
		if (EnemiesInWave == 0){


		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
