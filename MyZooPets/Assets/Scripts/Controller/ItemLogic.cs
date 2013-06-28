using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Item Logic Class
//Reference all Items.

public class ItemLogic : MonoBehaviour{
	
	//Each Item has its component kept in different lists.
	//This ID of the item is represented as index of the list. 
	public List<string> names = new List<string>();
	public List<Texture2D> textures = new List<Texture2D>();
	public List<int> costs = new List<int>();
	public List<Action> methods = new List<Action>();
	public List<ItemCategory> category = new List<ItemCategory>();
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
	
	private void categorize(){
		for(int i =0;i< items.Count;i++){
			if(items[i].Category == ItemCategory.Foods) foodlist.Add(i);
			if(items[i].Category == ItemCategory.Items) itemlist.Add(i);
			if(items[i].Category == ItemCategory.Inhalers) inhalerlist.Add(i);
			if(items[i].Category == ItemCategory.Decorations) decolist.Add(i);
		}
	}
	
	//This methos has to expand when more items added
	private void loadMethods(){
		methods.Add(()=>takeApple());
		methods.Add(()=>takeSandwich());
		methods.Add(()=>takeInhaler());
		methods.Add(()=>takeEmInhaler());	
	}
	
	void Awake(){
		if(names.Count == textures.Count && textures.Count == costs.Count){
			for(int i = 0;i< names.Count;i++){
				items.Add(new Item(i,names[i],costs[i],textures[i],category[i]));
			}
		}
		categorize();
		loadMethods();
	}

	//Functions for Each item. 
	public static void takeApple(){
		DataManager.AddHunger(10);
	}
	
	public static void takeSandwich(){
		DataManager.AddHunger(30);
	}
	
	public static void takeInhaler(){
		CalendarLogic.RecordGivingInhaler();
	}
	
	public static void takeEmInhaler(){
		
	}
}