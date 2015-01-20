using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

	public List<EnemyData> EnemyList;
	private List<GameObject> SpawnerList;
	public List <Wave> waves;
	public int EnemiesInWave=0;
	public int WaveNum=0;
	Wave waver;
	Wave CurrWave;
	public GameObject EnemyPrefab;

	public void reset(){
		if(SpawnerList != null){
		foreach (GameObject stuff in SpawnerList){
			Destroy(stuff);
		}
		SpawnerList.Clear();
		EnemyList.Clear();
		}
		EnemyList = new List<EnemyData>();
		SpawnerList = new List<GameObject>();
		waves = new List<Wave>();
		BuildEnemyList(DataLoaderTriggerArmy.GetDataList());

	}
	public void buildWaveList (List<ImmutableDataWaves> waveList){
		foreach(ImmutableDataWaves tsunami in waveList){
			waver = new Wave();
			waver.NumOfEnemies = tsunami.NumOfEnemies;
			waver.NumOfBasics= tsunami.BegEnemies;
			waver.NumOfMedium = tsunami.MediumEnemies;
			waver.NumOfHard= tsunami.HardEnemies;
			waves.Add(waver);
		}
		GenerateWave(0);
	}
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
	public void GenerateWave(int _WaveNum){
		Debug.Log("is");
		CurrWave=waves[_WaveNum];
		EnemiesInWave= int.Parse(waves[_WaveNum].NumOfEnemies);
		SpawnWave(CurrWave);
	}
	public void SpawnWave(Wave currWave){
		List<EnemyData> WaveEnemies;
		WaveEnemies = new List<EnemyData>();
		for (int i =0; i < EnemiesInWave; i++){
			WaveEnemies.Add(EnemyList[0]);
		}
		SpawnManager.instance.spawnTrigger(WaveEnemies);
		if(WaveNum >= waves.Count){
			WaveNum=0;
		}
		else{
			WaveNum++;
		}
	}
	// Update is called once per frame
	void Update () {
		if (EnemiesInWave == 0&&WaveNum!=0){
			GenerateWave(WaveNum);

		}
	}
}
