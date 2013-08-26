using UnityEngine;
using System.Collections;

//Item Model
//Contains: 1. name
//          2. textureName
//          3. cost
//          4. ID

[System.Serializable]
public class Item {
	public string name; //name of item
	public string textureName; //name of texture in the atlas
	public int cost; //cost of item
	public ItemCategory category; //item category
	public ItemReceiver itemreceiver; //who will be receiving the time
	public string description;
	
	public Item(string n, int c, string t, ItemCategory i)
	{
		this.name = n;
		this.cost = c;
		this.textureName = t;
		this.category = i;
	}
}
