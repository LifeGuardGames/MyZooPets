using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemLogic : MonoBehaviour{

	public List<string> names = new List<string>();
	public List<Texture2D> textures = new List<Texture2D>();
	public List<int> costs = new List<int>();
	
	private Hashtable items = new Hashtable();
	private Dictionary<object, Items.onUse> delegates;
		
	
	public Items getItem(string name){
		return (Items)items[name];
	}
	
	public void OnCall(string name){
		delegates[name]();
	}
	
	
	void Awake(){
		delegates = new Dictionary<object, Items.onUse>{
		{names[0], takeApple},
		{names[1], takeSandwich},
		{names[2], takeInhaler},
		{names[3], takeEmInhaler}
	};	
		
		if(names.Count == textures.Count && textures.Count == costs.Count){
			for(int i = 0;i< names.Count;i++){
				items.Add (names[i],new Items(names[i],costs[i],textures[i]));
			}
		}
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