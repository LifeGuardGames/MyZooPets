using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {

	public GameObject itemPrefab;
	public GameObject startLocation;
	public GameObject endLocation;

	private bool isRunning = false;
	private float frequency;
	public float Frequency{
		get{return frequency;}
		set{
			frequency = value;
			if(isRunning){	// Keep the track running if it is already active
				StartSpawning();
			}
		}
	}

	private float speed;
	public float Speed{
		get{return speed;}
		set{speed = value;}
	}

	public void StartSpawning(){
		StopSpawning();
		isRunning = true;
		InvokeRepeating("SpawnItem", 1f, frequency);
	}

	public void StopSpawning(){
		isRunning = false;
		CancelInvoke("SpawnItem");
	}
	
//	public void SetSpeed(float newSpeed){
//		speed = newSpeed;
//	}

//	public void SetFrequency(float newFrequency){
//		frequency = new frequence
//	}

	private void SpawnItem(){
		GameObject item = Instantiate(itemPrefab) as GameObject;
		item.transform.position = startLocation.transform.position;
		item.GetComponent<AssemblyLineItem>().SetupItem(this);
	}
}
