using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Item Logic Class
//Reference all Items.
//Each property of items are stored in a list, fill the list by draggin in Unity

//Item databases contains a list of all items
//Item ID = array index of the items list
//Methods list contains all functions for each item, cooresponding to its index
public class ItemLogic : MonoBehaviour{
	//This number has to change manually
	public static int MAX_ITEM_COUNT = 10;
	
	void Awake(){
		DataItems.SetupData();
	}

	public Item GetItem(string itemID, ItemType itemType){
		Item item;
		item = DataItems.GetItem(itemID, itemType);
		D.Assert(item != null, "itemID not valid");
		return item;
	}

	//Calls the id function in the function list.
	public void OnCall(int id){
	}

	private void LoadMethods(){
	}

}