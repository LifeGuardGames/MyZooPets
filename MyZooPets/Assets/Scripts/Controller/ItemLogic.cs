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
	
	//General item list. 
	public List<Item> items = new List<Item>();
	
	//This number has to change manually
	public static int MAX_ITEM_COUNT = 10;
	
	//Calls the id function in the function list.
	public void OnCall(int id){
		methods[id]();
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
				items.Add(new Item(i,names[i],costs[i],textures[i]));
			}
		}
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