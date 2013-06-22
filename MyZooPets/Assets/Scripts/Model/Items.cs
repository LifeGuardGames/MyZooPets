using UnityEngine;
using System.Collections;

public class Items {

	private string name;
	private Texture2D texture;
	private int cost;
	private int count;
	
	public Texture2D Texture{get{return texture;} }
	public string Name{get{return name;}}
	public int Count{get{return count;}}
	
	public Items(string n,int c, Texture2D t)
	{
		this.name = n;
		this.texture = null;
		this.cost = c;
		this.texture = t;
		this.count = 1;
	}
	
	public delegate void onUse();
}
