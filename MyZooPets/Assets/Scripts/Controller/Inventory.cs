using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
	Inventory class for Pet
	Contains all items the pet owns.
	Inventory creates space for all items at the beginning as an Array/
	Index of inventory array correspond to each item ID
	Int in each array position represents item count for each item
	eg.  inventory[2] = 1  means, the pet has 1 of the third item in Itemlogic class(the reference) 
*/
public class Inventory : MonoBehaviour {
	public static event EventHandler<InventoryEventArgs> OnItemAddedToInventory; //Call when an item is added
	public class InventoryEventArgs : EventArgs{
		public bool IsItemNew{get; set;}
		public int ItemID{get; set;}
	}

	private ItemLogic itemLogic;
	private int[] inventory ; //Use array to represent item. this way ID is same as index of the array.
	private int inventoryCount; //number of items that are actually in inventory
	//====================API===========================
	public int InventoryCount{
		get{return inventoryCount;}
	}
	public int[] InventoryArray{
		get{return inventory;}
	}

	//add items to inventory
	public void AddItem(int id, int count){
		InventoryEventArgs args = new InventoryEventArgs();
		args.ItemID = id;
		args.IsItemNew = false;
		if(inventory[id] == 0){ //add one to inventory Count if item is new
			inventoryCount++;
			args.IsItemNew = true;
		}
		inventory[id] += count;
		if(OnItemAddedToInventory != null) OnItemAddedToInventory(this, args);
	}
	
	//Use item from inventory
	public void UseItem(int id){
		if(inventory[id]!=0){
			inventory[id]--;
			itemLogic.OnCall(id);
		}
		if(inventory[id] == 0){ //minus one to inventory count if item is used up
			inventoryCount--;
		}
	}
	//=================================================
	
	// Use this for initialization
	void Awake () {
		// itemLogic =  GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
		// inventory = DataManager.Instance.Inventory.InventoryArray;
		// for(int i=0; i<itemLogic.items.Count; i++){
		// 	if(inventory[i] > 0) inventoryCount++;
		// }
	}
}
