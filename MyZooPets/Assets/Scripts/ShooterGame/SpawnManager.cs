using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {
	//first spawner
	public GameObject firstPos;
	//second spawner
	public GameObject secondPos;
	// third spawner
	public GameObject thirdPos;
	//list of positions to spawn enemy from
	List<Vector3> posList;
	// an enemy
	public GameObject enemy;
	// Use this for initialization
	void Start () {
		posList = new List<Vector3>();
		posList.Add(firstPos.transform.position);
		posList.Add (secondPos.transform.position);	
		posList.Add(thirdPos.transform.position);

	}
	
	public void spawnTrigger(int pos)
	{
		Instantiate(enemy,posList[pos],enemy.transform.rotation);
	}

}
