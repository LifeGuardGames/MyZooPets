using UnityEngine;
using System.Collections;

public class StoreGUI : MonoBehaviour {

	public Texture2D backgroundTexture;
	public Texture2D page1Texture,page2Texture,page3Texture,page4Texture;
	
	private Vector2 page1loc = new Vector2(100,100);
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(0,0,1000,700),backgroundTexture);
		GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,800,600),page1Texture);
	}
}
