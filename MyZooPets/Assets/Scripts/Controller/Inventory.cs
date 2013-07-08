using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Inventory class for Pet
//Contains all items the pet owns.
//Inventory creates space for all items at the beginning as an Array/
//Index of inventory array correspond to each item ID
//Int in each array position represents item count for each item
//  eg.  inventory[2] = 1  means, the pet has 1 of the third item in Itemlogic class(the reference) 
public class Inventory : MonoBehaviour {
	private ItemLogic itemlogic;
	private int[] inventory ; //Use array to represent item. this way ID is same as index of the array.
	
	public bool isDebug; //developing option
	public int[] InventoryArray{
		get{return inventory;}
	}

	//add items to inventory
	public void addItem(int id, int count){
		inventory[id] += count;
	}
	
	//Use item from inventory
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
		addItem(0,2); //2 apples
		addItem(1,1); //1 sandwich
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
