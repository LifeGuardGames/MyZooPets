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
	//Each Item has its component kept in different lists.
	private List<Action> methods = new List<Action>(); //List of actions to be called when item is used. 
														//Index: itemID, Value: functions
	private List<int> foodList = new List<int>(); //Index: regular array index, Value: itemID 
	private List<int> itemList = new List<int>(); //Index: regular array index, Value: itemID 
	private List<int> inhalerList = new List<int>(); //Index: regular array index, Value: itemID
	private List<int> decoList = new List<int>(); //Index: regular array index, Value: itemID 
	public List<Item> items = new List<Item>(); //item database
												//Index: itemID, Value: instance of Item class
	//============Getters=============	
	public List<int> FoodList{
		get{return foodList;}
	}	
	public List<int> ItemList{
		get{return itemList;}
	}
	public List<int> InhalerList{
		get{return inhalerList;}
	}
	public List<int> DecoList{
		get{return decoList;}
	}
	public List<Item> Items{
		get{return items;}
	}
	//===============================

	//This number has to change manually
	public static int MAX_ITEM_COUNT = 10;
	
	//Calls the id function in the function list.
	public void OnCall(int id){
		methods[id]();
	}
	
	//sorting items list into category list
	private void Categorize(){
		for(int i =0;i< items.Count;i++){
			if(items[i].category == ItemCategory.Foods) foodList.Add(i);
			if(items[i].category == ItemCategory.Items) itemList.Add(i);
			if(items[i].category == ItemCategory.Inhalers) inhalerList.Add(i);
			if(items[i].category == ItemCategory.Decorations) decoList.Add(i);
		}
	}
	
	void Awake(){
		//initalize all item in the database. Add methods and description
		Categorize();
		LoadMethods();
	}

	private void LoadMethods(){
		methods.Add(()=>TakeApple());
		methods.Add(()=>TakeGreenApple());
		methods.Add(()=>TakeSandwich());
		methods.Add(()=>TakeBread());
		methods.Add(()=>TakeDoughnut());
		methods.Add(()=>TakeDoughnutBrown());
		methods.Add(()=>TakeMilk());
	}

	//Functions for Each item.
	private void TakeApple(){
		DataManager.AddMood(10);
	}
	private void TakeGreenApple(){
		DataManager.AddMood(5);
	}
	private void TakeSandwich(){
		DataManager.AddMood(30);
	}
	private void TakeBread(){
		DataManager.AddMood(15);
	}
	private void TakeDoughnut(){
		DataManager.AddMood(20);
	}
	private void TakeDoughnutBrown(){
		DataManager.AddMood(25);
	}
	private void TakeMilk(){
		DataManager.AddMood(40);
	}
}