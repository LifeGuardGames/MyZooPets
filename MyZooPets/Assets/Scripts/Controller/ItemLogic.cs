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
		methods.Add(()=>TakeSandwich());
		methods.Add(()=>UseCarpet(items[2].texture));
		methods.Add(()=>UseCarpet(items[3].texture));
		methods.Add(()=>UseCarpet(items[4].texture));
	}
	
	private void AddDescription(){
		items[0].description = "Health + 50";
		items[1].description = "Health + 100";
		items[2].description = "Cute Teddy";
		items[3].description = "Brand New!";
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
	
	public static void TakeSandwich(){
		DataManager.AddMood(30);
	}
	
	public static void UseCarpet(Texture texture){
		GameObject.Find("Floor Rectangular").renderer.material.SetTexture("_MainTex",texture);
	}
}