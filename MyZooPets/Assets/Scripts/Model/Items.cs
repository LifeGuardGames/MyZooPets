using UnityEngine;
using System.Collections;

//Item Model
//Contains: 1. name
//          2. texture
//          3. cost
//          4. ID

[System.Serializable]
public class Item {

	public string name;
	public Texture2D texture;
	public int cost;
	public ItemCategory category;
	public ItemReceiver itemreceiver;
	public string description;
	
	public ItemCategory Category{get{return category;}}
	public int ID;
	public Texture2D Texture{get{return texture;} }
	public string Name{get{return name;}}
	public int Cost{get{return cost;}}
	
	public Item(int id,string n,int c, Texture2D t, ItemCategory i)
	{
		this.ID = id;
		this.name = n;
		this.cost = c;
		this.texture = t;
		this.category = i;
	}
}
