using UnityEngine;
using System.Collections;

//Item Model
//Contains: 1. name
//          2. texture
//          3. cost
//          4. ID


public class Item {

	private string name;
	private Texture2D texture;
	private int cost;
	private ItemCategory category;
	
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
