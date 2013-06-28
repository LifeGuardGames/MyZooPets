using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Inventory class for Pet
public class Inventory : MonoBehaviour {

	
	//Use array to represent item. 
	//this way ID is same as index of the array.
	public int[] inventory ;
	public bool isDebug;
	private ItemLogic itemlogic;
	
	public void addItem(int id, int count){
		inventory[id] += count;
	
	}
	
	public void useItem(int id){
		if(inventory[id]!=0){
			inventory[id] --;
			itemlogic.OnCall(id);
		}
	}
	
	// Use this for initialization
	void Start () {
		itemlogic =  GameObject.Find("GameManager").GetComponent<ItemLogic>();
		inventory = DataManager.Inventory;
		if(isDebug){
			inventory = new int[10];
		}
		
		//testing
		addItem(1,2); //2 apples
		addItem(2,1); //1 sandwich
		addItem(3,1); //1 inhaler
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
