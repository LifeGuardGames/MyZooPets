using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Item Logic Class
//Reference all Items.
//Each property of items are stored in a list, fill the list by draggin in Unity
//General list items contains a list of all items
//Each Item 's ID is the index of itself in the general items list. 
//Methods list contains all funtions for each item, cooresponding to its index


public class ItemLogic : MonoBehaviour{
	
	//Each Item has its component kept in different lists.
	//This ID of the item is represented as index of the list. 
//	public List<string> names = new List<string>();
//	public List<Texture2D> textures = new List<Texture2D>();
//	public List<int> costs = new List<int>();
	public List<Action> methods = new List<Action>();
//	public List<ItemCategory> category = new List<ItemCategory>();
	
	public List<int> foodlist = new List<int>();
	public List<int> itemlist = new List<int>();
	public List<int> inhalerlist = new List<int>();
	public List<int> decolist = new List<int>();

	//General item list. 
	public List<Item> items = new List<Item>();
	
	//This number has to change manually
	public static int MAX_ITEM_COUNT = 10;
	
	//Calls the id function in the function list.
	public void OnCall(int id){
		methods[id]();
	}
	
	private void Categorize(){
		for(int i =0;i< items.Count;i++){
			if(items[i].Category == ItemCategory.Foods) foodlist.Add(i);
			if(items[i].Category == ItemCategory.Items) itemlist.Add(i);
			if(items[i].Category == ItemCategory.Inhalers) inhalerlist.Add(i);
			if(items[i].Category == ItemCategory.Decorations) decolist.Add(i);
		}
	}
	
	//This methos has to expand when more items added
	private void LoadMethods(){
		methods.Add(()=>TakeApple());
		methods.Add(()=>TakeGreenApple());
		methods.Add(()=>TakeSandwich());
		methods.Add(()=>TakeBread());
		methods.Add(()=>TakeDoughnut());
		methods.Add(()=>TakeDoughnutBrown());
		methods.Add(()=>TakeMilk());
		
	}
	
	private void AddDescription(){
		items[0].description = "Mood + 10";
		items[1].description = "Mood + 5";
		items[2].description = "Mood + 30";
		items[3].description = "Mood + 15";
		items[4].description = "Mood + 20";
		items[5].description = "Mood + 25";
		items[6].description = "Mood + 40";
	}
	
	void Awake(){
//		if(names.Count == textures.Count && textures.Count == costs.Count){
//			for(int i = 0;i< names.Count;i++){
//				items.Add(new Item(i,names[i],costs[i],textures[i],category[i]));
//			}
//		}
		Categorize();
		LoadMethods();
		AddDescription();
	}

	//Functions for Each item.
	public static void TakeApple(){
		DataManager.AddMood(10);
	}
	public static void TakeGreenApple(){
		DataManager.AddMood(5);
	}
	public static void TakeSandwich(){
		DataManager.AddMood(30);
	}
	public static void TakeBread(){
		DataManager.AddMood(15);
	}
	public static void TakeDoughnut(){
		DataManager.AddMood(20);
	}
	public static void TakeDoughnutBrown(){
		DataManager.AddMood(25);
	}
	public static void TakeMilk(){
		DataManager.AddMood(40);
	}
	
	//Template for Room Decoration
//	public static void UseCarpet(Texture texture){
//		GameObject.Find("Floor Rectangular").renderer.material.SetTexture("_MainTex",texture);
//	}
}