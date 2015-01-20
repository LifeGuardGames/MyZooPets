using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

	private List<Enemy> EnemyList;
	private List<GameObject> SpawnerList;
	private List <Wave> waves;
	public int EnemiesInWave;
	Wave waver;
	Wave CurrWave;

	public void reset(){
		if(SpawnerList != null){
		foreach (GameObject stuff in SpawnerList){
			Destroy(stuff);
		}
		
		SpawnerList.Clear();
		EnemyList.Clear();
		}
		EnemyList = new List<Enemy>();
		SpawnerList = new List<GameObject>();
		waves = new List<Wave>();
		BuildEnemyList(DataLoaderTriggerArmy.GetDataList());
	}
	public void buildWaveList (List<ImmutableDataWaves> waveList){
		foreach(ImmutableDataWaves tsunami in waveList){
			waver.NumOfEnemies = tsunami.NumOfEnemies;
			waver.NumOfBasics= tsunami.BegEnemies;
			waver.NumOfMedium = tsunami.MediumEnemies;
			waver.NumOfHard= tsunami.HardEnemies;
			waves.Add(waver);
		}

	}
	public void BuildEnemyList(List<ImmutableDataTriggerArmy> mobList){
		foreach (ImmutableDataTriggerArmy baddie in mobList){
			Enemy mob =  new Enemy();
			mob.name = baddie.Name;
			mob.spritz = baddie.SpriteName;
			mob.AiScript= baddie.AI;
			EnemyList.Add(mob);
		}
	}
	public void GenerateWave(int WaveNum){
		CurrWave=waves[WaveNum];
	}
	public void SpawnWave(Wave CurrWave){
		Enemy[] WaveEnemies;
		WaveEnemies = new Enemy[EnemiesInWave];
		for (int i =0; i < EnemiesInWave; i++){
			WaveEnemies[i] = EnemyList[i];
		}
		SpawnManager.instance.spawnTrigger(WaveEnemies);
	}
	// Update is called once per frame
	void Update () {

	}
}
