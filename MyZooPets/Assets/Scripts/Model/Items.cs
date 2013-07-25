using UnityEngine;
using System.Collections;

//Item Model
//Contains: 1. name
//          2. texture
//          3. cost
//          4. ID

[System.Serializable]
public class Item {
	public string name; //name of item
	public Texture2D texture; //texture of item
	public int cost; //cost of item
	public ItemCategory category; //item category
	public ItemReceiver itemreceiver; //who will be receiving the time
	public string description;
	
	public Item(string n,int c, Texture2D t, ItemCategory i)
	{
		this.name = n;
		this.cost = c;
		this.texture = t;
		this.category = i;
	}
}
