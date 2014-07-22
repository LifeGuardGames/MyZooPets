using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {

	public GameObject itemPrefab;
	public GameObject startLocation;
	public GameObject endLocation;
//	public List<AssemblyLineItem> itemList;	// TODO Need to use listener

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
//		foreach(AssemblyLineItem item in itemList){
//			item.SetSpeed(newSpeed);
//		}
	}

	private void SpawnItem(){
		GameObject item = Instantiate(itemPrefab) as GameObject;
		item.transform.position = startLocation.transform.position;
		item.GetComponent<AssemblyLineItem>().SetupItem(this);
	}
}
