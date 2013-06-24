using UnityEngine;
using System.Collections;

public class Item {

	private string name;
	private Texture2D texture;
	private int cost;
	private int count;
	
	public int ID;
	public Texture2D Texture{get{return texture;} }
	public string Name{get{return name;}}
	
	public Item(int id,string n,int c, Texture2D t)
	{
		this.ID = id;
		this.name = n;
		this.cost = c;
		this.texture = t;
	}
}
