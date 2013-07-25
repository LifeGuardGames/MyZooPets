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
		
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 10, Vector3.zero);
	}
	private void TakeGreenApple(){
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 5, Vector3.zero);
	}
	private void TakeSandwich(){
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 30, Vector3.zero);
	}
	private void TakeBread(){
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 15, Vector3.zero);
	}
	private void TakeDoughnut(){
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 20, Vector3.zero);
	}
	private void TakeDoughnutBrown(){
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 25, Vector3.zero);
	}
	private void TakeMilk(){
		// TODO-j TEMPORARY PLEASE CHANGE DATAMANAGER SINGLETON
		GameObject data = GameObject.Find("GameManager");
		StatsController control = data.GetComponent<StatsController>();
		control.ChangeStats(0, 0, 0, 40, Vector3.zero);
	}
}