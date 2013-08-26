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
	public List<Item> items = new List<Item>(); //item database
												//Index: itemID, Value: instance of Item class

	private List<Action> methods = new List<Action>(); //List of actions to be called when item is used. 
														//Index: itemID, Value: functions
	private List<int> foodList = new List<int>(); //Index: regular array index, Value: itemID 
	private List<int> itemList = new List<int>(); //Index: regular array index, Value: itemID 
	private List<int> inhalerList = new List<int>(); //Index: regular array index, Value: itemID
	private List<int> decoList = new List<int>(); //Index: regular array index, Value: itemID 

	public List<int> FoodList{get{return foodList;}}	
	public List<int> ItemList{get{return itemList;}}
	public List<int> InhalerList{get{return inhalerList;}}
	public List<int> DecoList{get{return decoList;}}
	public List<Item> Items{get{return items;}}

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
		methods.Add(()=>TakeInhaler());
	}

	//Functions for Each item.
	private void TakeApple(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 10, Vector3.zero);
	}
	private void TakeGreenApple(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 5, Vector3.zero);
	}
	private void TakeSandwich(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 30, Vector3.zero);
	}
	private void TakeBread(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 15, Vector3.zero);
	}
	private void TakeDoughnut(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 20, Vector3.zero);
	}
	private void TakeDoughnutBrown(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 25, Vector3.zero);
	}
	private void TakeMilk(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 40, Vector3.zero);
	}
	private void TakeInhaler(){
		StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 10, Vector3.zero, 0, Vector3.zero);
	}
}