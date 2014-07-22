using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {

	public GameObject itemPrefab;
	public GameObject startLocation;
	public GameObject endLocation;

//	private float interval;
//	public float Interval{
//		get{return interval;}
//		set{interval = value;}
//	}

	private float speed;
	public float Speed{
		get{return speed;}
		set{speed = value;}
	}

	public void StartSpawning(){
		InvokeRepeating("SpawnItem", 1f, 1.5f);
	}

	public void StopSpawning(){
		CancelInvoke();
	}
	
	public void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

	private void SpawnItem(){
		GameObject item = Instantiate(itemPrefab) as GameObject;
		item.transform.position = startLocation.transform.position;
		item.GetComponent<AssemblyLineItem>().SetupItem(this);
	}
}
