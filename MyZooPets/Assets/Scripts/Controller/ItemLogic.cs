using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemLogic : MonoBehaviour{

	public List<string> names = new List<string>();
	public List<Texture2D> textures = new List<Texture2D>();
	public List<int> costs = new List<int>();
	public List<Item> items = new List<Item>();
	public List<Action> methods = new List<Action>();
	
	
	public void OnCall(int id){
		methods[id]();
	}
	
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