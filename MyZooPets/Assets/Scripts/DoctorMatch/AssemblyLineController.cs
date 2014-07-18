using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssemblyLineController : MonoBehaviour {

	public GameObject itemPrefab;
	public GameObject startLocation;
	public GameObject endLocation;
	public List<AssemblyLineItem> itemList;

	private float speed;
	public float Speed{
		get{return speed;}
		set{speed = value;}
	}

	void Start(){
		Speed = 4.0f;
//		StartSpawning();
	}

	public void StartSpawning(){
		InvokeRepeating("SpawnItem", 1f, 1f);
	}

	public void SetSpeed(float newSpeed){
		speed = newSpeed;
		foreach(AssemblyLineItem item in itemList){
			item.SetSpeed(newSpeed);
		}
	}

	void Reset(){

	}

	private void SpawnItem(){
		GameObject item = Instantiate(itemPrefab) as GameObject;
		item.transform.position = startLocation.transform.position;
		item.GetComponent<AssemblyLineItem>().SetupItem(this);
	}
}
