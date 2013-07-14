using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreNGUI : MonoBehaviour {
	
	public GameObject ItemPrefab;
	
	// Use this for initialization
	void Start () {
		GameObject item = (GameObject)Instantiate(ItemPrefab);
		item.SetParent(GameObject.Find ("Grid"));
		item.name = "haha";
		
		item = (GameObject)Instantiate(ItemPrefab);
		item.name = "jj";
		item.SetParent(GameObject.Find ("Grid"));
		GameObject.Find("Grid").GetComponent<UIGrid>().Reposition();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void lala(GameObject a){
		print (a);
//		print (a);
		print ("lalalal");
	}
	
	void haha(){
		
	}
}
