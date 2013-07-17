using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


//Inventory class for Pet
//Contains all items the pet owns.
//Inventory creates space for all items at the beginning as an Array/
//Index of inventory array correspond to each item ID
//Int in each array position represents item count for each item
//  eg.  inventory[2] = 1  means, the pet has 1 of the third item in Itemlogic class(the reference) 
public class Inventory : MonoBehaviour {
	private ItemLogic itemlogic;
	private int[] inventory ; //Use array to represent item. this way ID is same as index of the array.
	private int inventoryCount; //number of items that are actually in inventory
	public bool isDebug; //developing option

	//====================API===========================
	public int InventoryCount{
		get{return inventoryCount;}
	}
	public int[] InventoryArray{
		get{return inventory;}
	}

	//add items to inventory
	public void AddItem(int id, int count){
		if(inventory[id] == 0){ //add one to inventory Count if item is new
			inventoryCount++;
		}
		inventory[id] += count;
	}
	
	//Use item from inventory
	public void UseItem(int id){
		if(inventory[id]!=0){
			inventory[id]--;
			itemlogic.OnCall(id);
		}
		if(inventory[id] == 0){ //minus one to inventory count if item is used up
			inventoryCount--;
		}
	}
	//=================================================
	
	// Use this for initialization
	void Awake () {
		itemlogic =  GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
		inventory = DataManager.Inventory;
		if(isDebug){
			inventory = new int[10];
			AddItem(0,2); //2 apples
			AddItem(1,1); //1 sandwich
		}
	}
}
