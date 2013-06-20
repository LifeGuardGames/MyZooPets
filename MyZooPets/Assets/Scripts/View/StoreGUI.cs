using UnityEngine;
using System.Collections;

public class StoreGUI : MonoBehaviour {

	public Texture2D backgroundTexture;
	public Texture2D page1Texture,page2Texture,page3Texture,page4Texture;
	
	private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	private Vector2 backgroundloc = new Vector2(40,20);
	private Vector2 page1loc = new Vector2(100,100);
	
	private int storePage = 1;
	private bool StoreGUIOn = false;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void showStore(){
		StoreGUIOn = true;
	}
	
	public void hideStore(){
		StoreGUIOn = false;
	}
	
	void OnGUI(){
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
		
		if(StoreGUIOn){
			GUI.DrawTexture(new Rect(backgroundloc.x,backgroundloc.y,1200,760),backgroundTexture);
			if(GUI.Button(new Rect (page1loc.x+50,page1loc.y-50,200,50),"page1")){
				storePage = 1;
			}
			if(GUI.Button(new Rect (page1loc.x+300,page1loc.y-50,200,50),"page2")){
				storePage = 2;
			}
			if(GUI.Button(new Rect (page1loc.x+550,page1loc.y-50,200,50),"page3")){
				storePage = 3;
			}
			if(GUI.Button(new Rect (page1loc.x+800,page1loc.y-50,200,50),"page4")){
				storePage = 4;
			}
			
			if(storePage == 1){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page1Texture);
			}
			if(storePage == 2){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page2Texture);
			}
			if(storePage == 3){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page3Texture);
			}
			if(storePage == 4){
				GUI.DrawTexture(new Rect(page1loc.x,page1loc.y,1080,650),page4Texture);
			}
			
			if(GUI.Button(new Rect(backgroundloc.x+50,backgroundloc.y+50,50,50),"X")){
				hideStore();
			}
		}
	}
}
